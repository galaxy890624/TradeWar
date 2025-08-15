using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

namespace galaxy890624
{
    /// <summary>
    /// 控制建築放置流程：預覽、旋轉、確認建造、取消建造
    /// </summary>
    public class BuildPlacerTest : MonoBehaviour
    {
        public static BuildPlacerTest Instance { get; private set; }

        [Header("建築放置設定")]
        public LayerMask groundMask;
        public Material previewMaterial;

        [Header("建築攝影機與 UI 控制")]
        public CinemachineVirtualCamera buildCamera;
        public CinemachineVirtualCamera playerCamera;
        public GameObject buildingUIRoot;

        private GameObject currentPreview;
        private BuildingData currentData;
        private float currentRotation = 0f;

        private void Awake()
        {
            Instance = this;

            // 初始狀態下 UI 與建築相機為關閉
            if (buildingUIRoot != null) buildingUIRoot.SetActive(false);
            if (buildCamera != null) buildCamera.gameObject.SetActive(false);
        }

        private void Update()
        {
            // 按下 F7 進入建造模式（僅用作測試入口）
            if (Input.GetKeyDown(KeyCode.F7) && currentData != null)
            {
                Debug.Log("[BuildPlacerTest.cs] F7");
                StartPlacing(currentData);
            }

            if (currentPreview == null) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask))
            {
                MapTile startTile = hit.collider.GetComponent<MapTile>();
                if (startTile == null) return;

                int startRow = startTile.row;
                int startCol = startTile.col;

                List<MapTile> occupiedTiles = GetTilesToOccupy(startRow, startCol, currentData.width, currentData.height);
                bool canPlace = occupiedTiles.Count == currentData.width * currentData.height &&
                                occupiedTiles.TrueForAll(t => !t.IsOccupied);

                currentPreview.transform.position = startTile.transform.position + Vector3.up * 0.5f;
                currentPreview.transform.rotation = Quaternion.Euler(0, currentRotation, 0);

                if (Input.GetMouseButtonDown(0) && canPlace)
                {
                    PlaceBuilding(currentData, occupiedTiles, currentRotation);
                    EndPlacing();
                }
            }

            // 滾輪旋轉
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                float step = Input.GetKey(KeyCode.R) ? 15f : 90f;
                currentRotation += scroll > 0 ? step : -step;
            }

            // 右鍵取消
            if (Input.GetMouseButtonDown(1))
            {
                EndPlacing();
            }
        }

        /// <summary>
        /// 由 UI 或其他腳本觸發建築開始（顯示 UI / 鏡頭）
        /// </summary>
        public void StartPlacing(BuildingData data)
        {
            currentData = data;

            // 切換鏡頭與 UI
            if (playerCamera != null) playerCamera.gameObject.SetActive(false);
            if (buildCamera != null) buildCamera.gameObject.SetActive(true);
            if (buildingUIRoot != null) buildingUIRoot.SetActive(true);

            if (currentPreview != null) Destroy(currentPreview);
            currentPreview = Instantiate(data.prefab);
            ApplyPreviewMaterial(currentPreview);
            currentRotation = 0f;
        }

        /// <summary>
        /// 完成建築流程：關閉預覽與 UI
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
        /// 產生實際建築物
        /// </summary>
        public void PlaceBuilding(BuildingData data, List<MapTile> tiles, float rotationY = 0f)
        {
            if (tiles == null || tiles.Count == 0) return;

            Vector3 position = tiles[0].transform.position + Vector3.up * 0.5f;
            Quaternion rotation = Quaternion.Euler(0, rotationY, 0);

            GameObject building = Instantiate(data.prefab, position, rotation);
            Building buildingComponent = building.GetComponent<Building>();
            if (buildingComponent == null)
            {
                buildingComponent = building.AddComponent<Building>();
            }

            buildingComponent.Init(data, tiles.ToArray());

            foreach (var tile in tiles)
            {
                tile.IsOccupied = true;
                tile.CurrentBuilding = buildingComponent.gameObject;
            }
        }

        /// <summary>
        /// 從起點取得占用範圍的所有格子
        /// </summary>
        private List<MapTile> GetTilesToOccupy(int startRow, int startCol, int width, int height)
        {
            List<MapTile> result = new();
            foreach (MapTile tile in FindObjectsOfType<MapTile>())
            {
                if (tile.row >= startRow && tile.row < startRow + height &&
                    tile.col >= startCol && tile.col < startCol + width)
                {
                    result.Add(tile);
                }
            }
            return result;
        }

        /// <summary>
        /// 替預覽建築加上透明材質
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