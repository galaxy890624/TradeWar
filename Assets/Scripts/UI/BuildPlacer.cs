using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace galaxy890624
{
    /// <summary>
    /// 放置控制器 <br></br><br></br>
    /// 控制放置流程（預覽、旋轉、確認建造）<br></br>
    /// 控制玩家當前的建造狀態 <br></br>
    /// 建築時切換到俯視相機並顯示 UI <br></br>
    /// </summary>
    public class BuildPlacer : MonoBehaviour
    {
        public static BuildPlacer Instance { get; private set; }

        [Header("建築放置設定")]
        public LayerMask groundMask;
        public Material previewMaterial;

        [Header("建築攝影機與 UI 控制")]
        public CinemachineVirtualCamera buildCamera; // 俯視建築相機
        public CinemachineVirtualCamera playerCamera; // 玩家控制的原本相機
        public GameObject buildingUIRoot; // 整個"建築選單UI"的總容器,也就是建築按鈕 ( 例如 : 住宅 Lv1、交易所 Lv2、研究所 Lv1 ) 的父物件

        private GameObject currentPreview;
        private BuildingData currentData;
        private float currentRotation = 0f;

        /// <summary>
        /// 事件系統 <br><br></br></br>
        /// 1. 當按下蓋建築鍵的時候, 要做的事情 <br></br>
        /// 2. 切換攝影機 : 玩家的攝影機 -> MapManager的攝影機 <br></br>
        /// </summary>
        private Event EventSystem;

        private void Awake()
        {
            Instance = this;

            // 初始化時關閉建築 UI 和建築相機
            if (buildingUIRoot != null) buildingUIRoot.SetActive(false);
            if (buildCamera != null) buildCamera.gameObject.SetActive(false);
        }
        /// <summary>
        /// 開始建築流程 : 由 UI 按鈕觸發 <br><br></br></br>
        /// 點擊建築選單按鈕 → 開始建築預覽模式 <br></br>
        /// </summary>
        public void StartPlacing(BuildingData data)
        {
            currentData = data;

            // 啟用建築模式 : 切換鏡頭與 UI
            if (playerCamera != null) playerCamera.gameObject.SetActive(false);
            if (buildCamera != null) buildCamera.gameObject.SetActive(true);
            if (buildingUIRoot != null) buildingUIRoot.SetActive(true);

            // 如果有之前的預覽建築, 就清除掉
            if (currentPreview != null) Destroy(currentPreview);

            // 產生新的建築預覽模型
            currentPreview = Instantiate(data.prefab);
            ApplyPreviewMaterial(currentPreview);
            currentRotation = 0f;
        }

        void Update()
        {
            if (currentPreview == null) return;

            // 滑鼠點擊地格 (左上角起點)
            // 滑鼠發射 Ray 偵測地格
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask))
            {
                MapTile startTile = hit.collider.GetComponent<MapTile>();
                if (startTile == null) return;

                // 找出起點 row/col
                int startRow = startTile.row;
                int startCol = startTile.col;

                // 嘗試取得該建築將佔用的所有格子
                // 從左上角 tile 擴展出要佔用的所有 tile
                List<MapTile> occupiedTiles = GetTilesToOccupy(startRow, startCol, currentData.width, currentData.height);

                // 檢查是否每個格子都存在且未被佔用
                // 若有成功取得且都沒被佔用 → 可建
                bool canPlace = occupiedTiles.Count == currentData.width * currentData.height &&
                                occupiedTiles.TrueForAll(t => !t.IsOccupied);

                // 預覽位置放在左上格
                // 預覽位置在左上 tile 上 + 浮起 0.5 單位
                currentPreview.transform.position = startTile.transform.position + Vector3.up * 0.5f;
                currentPreview.transform.rotation = Quaternion.Euler(0, currentRotation, 0);

                // 滑鼠左鍵放置建築
                if (Input.GetMouseButtonDown(0) && canPlace)
                {
                    PlaceBuilding(currentData, occupiedTiles, currentRotation);
                    EndPlacing();
                }
            }

            // 滾輪旋轉
            // 滾輪旋轉建築（按住 R 鍵變成 15 度微調）
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                float step = Input.GetKey(KeyCode.R) ? 15f : 90f;
                currentRotation += scroll > 0 ? step : -step;
            }

            // 滑鼠右鍵取消放置
            if (Input.GetMouseButtonDown(1)) EndPlacing();
        }

        /// <summary>
        /// 結束建築流程 : 清除預覽、還原 UI 與攝影機。
        /// </summary>
        private void EndPlacing()
        {
            if (currentPreview != null) Destroy(currentPreview);
            currentPreview = null;
            currentData = null;

            if (playerCamera != null) playerCamera.gameObject.SetActive(true);
            if (buildCamera != null) buildCamera.gameObject.SetActive(false);
            if (buildingUIRoot != null) buildingUIRoot.SetActive(false);
        }

        /// <summary>
        /// 建立建築物在指定的 tiles 區域 <br></br>
        /// 可供升級、載入、編輯器產生等用途 <br></br>
        /// </summary>
        public void PlaceBuilding(BuildingData data, List<MapTile> tiles, float rotationY = 0f)
        {
            if (tiles == null || tiles.Count == 0) return;

            Vector3 position = tiles[0].transform.position + Vector3.up * 0.5f;
            Quaternion rotation = Quaternion.Euler(0, rotationY, 0);

            GameObject building = Instantiate(data.prefab, position, rotation);
            Building buildingComponent = building.AddComponent<Building>();
            buildingComponent.Init(data, tiles.ToArray());

            foreach (var tile in tiles)
            {
                tile.IsOccupied = true;
                tile.CurrentBuilding = building;
            }
        }

        /// <summary>
        /// 取得從某起點開始，向右下延展的 tile 區塊 <br></br>
        /// 找出從左上角開始，寬×高 的所有格子 <br></br>
        /// </summary>
        private List<MapTile> GetTilesToOccupy(int startRow, int startCol, int width, int height)
        {
            List<MapTile> result = new();

            foreach (MapTile tile in FindObjectsOfType<MapTile>())
            {
                if (tile.row >= startRow && tile.row < startRow + height && tile.col >= startCol && tile.col < startCol + width) result.Add(tile);
            }

            return result;
        }

        /// <summary>
        /// 將預覽建築套用透明材質 <br></br>
        /// </summary>
        private void ApplyPreviewMaterial(GameObject obj)
        {
            foreach (var renderer in obj.GetComponentsInChildren<Renderer>())
            {
                renderer.material = previewMaterial;
            }
        }
    }

}


// 建議搭配的 Unity UI 結構 : 
// - Canvas
//   - Panel (靠右固定定位，Vertical Layout Group)
//     - ButtonPrefab (含 icon + name + 建立事件)
// - 所有格子有 Collider + MapTile.cs，需加 tag/layer 給 Raycast
