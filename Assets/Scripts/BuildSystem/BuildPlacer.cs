using Cinemachine;                       // 使用 Cinemachine 的 VirtualCamera 類別
using System.Collections.Generic;         // 使用 List<T>
using System.Linq;                        // 方便做 LINQ（如果需要）
using UnityEngine;                        // Unity API

namespace galaxy890624
{
    /// <summary>
    /// BuildPlacer：改良版的建築放置控制器
    /// - 處理 preview、射線、可放置檢查、旋轉、以及高亮格子
    /// </summary>
    public class BuildPlacer : MonoBehaviour
    {
        // ---------- 公開欄位（Inspector 可調） ----------
        public static BuildPlacer Instance { get; private set; }    // Singleton 方便其他系統呼叫

        [Header("建築放置設定")]
        public LayerMask groundMask;                                // 判定地面(Tile)的 LayerMask（必須包含你的 Tile Layer）
        public Material previewMaterial;                            // （可選）統一 preview 用材質；若為 null 我們會直接修改 prefab 的 material instance

        [Header("相機與 UI 控制")]
        public CinemachineVirtualCamera buildCamera;                // 建造模式相機（可選）
        public CinemachineVirtualCamera playerCamera;               // 遊戲模式相機（可選）
        public GameObject buildingUIRoot;                           // 建造相關 UI 根物件（進入建造顯示）
        [Tooltip("若場景中有多個 Camera，可手動指定；否則會使用 Camera.main")]
        public Camera mainCamera;                                   // 主相機（Raycast 傳入）

        [Header("測試用建築資料 (F7 啟動)")]
        public BuildingData testData;                               // 測試用 BuildingData，按 F7 開始建造

        [Header("預覽疊加顏色")]
        [SerializeField] private Color canPlaceOverlay = new Color(0f, 1f, 0f, 0.45f);   // 可放置時的疊加顏色（綠）
        [SerializeField] private Color cannotPlaceOverlay = new Color(1f, 0f, 0f, 0.45f); // 不能放置時的疊加顏色（紅）

        [Header("除錯（打開會看到更多 console 訊息）")]
        public bool debugLogs = false;                              // 控制是否印詳細 Log（避免大量輸出）

        [SerializeField, Header("PlayerData的ScriptableObj")] private PlayerData playerData; // 玩家資料（可選）

        // ---------- 私有欄位（內部狀態） ----------
        private GameObject currentPreview;                          // 當前的預覽物件（玩家還沒放的那個）
        private BuildingData currentData;                           // 當前在放置的建築資料
        private float currentRotation;                              // 當前預覽的 Y 軸旋轉（度數）
        private List<MapTile> highlightedTiles = new();             // 目前被高亮的 tiles（用於還原顏色）

        // ---------- Unity 事件：Awake ----------
        private void Awake()
        {
            // 設定 Singleton 實例（簡單實作）
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // 初始 camera：若 Inspector 未指定，就使用 Camera.main
            if (mainCamera == null) mainCamera = Camera.main;

            // 如果有連 UI，預設隱藏（避免一開始就顯示）
            if (buildingUIRoot != null) buildingUIRoot.SetActive(false);

            // 預設關掉建造相機
            if (buildCamera != null) buildCamera.gameObject.SetActive(false);

            if (debugLogs) Debug.Log("<color=#ff00ff>[BuildPlacer.cs] Awake 完成</color>");
        }

        // ---------- 公開方法：由外部呼叫開始放置（UI 按鈕、F7 等） ----------
        public void StartPlacing(BuildingData data)
        {
            // 設定要放的建築資料
            currentData = data;
            /*if (currentData == null)
            {
                Debug.LogWarning("<color=#ff00ff>[BuildPlacer.cs] StartPlacing 被呼叫但 data 為 null</color>");
                return;
            }*/

            // 切換相機到建造模式並顯示 UI
            SwitchCamera(true);

            // 若有舊的 preview，刪掉
            // if (currentPreview != null) Destroy(currentPreview);

            // 實體化一個預覽物件（用 prefab 的複本）
            if(playerData.Wood >= currentData.costWood) currentPreview = Instantiate(data.prefab);

            if (currentPreview == null)
            {
                Debug.LogError("<color=#ff00ff>[BuildPlacer.cs] Instantiate Preview 失敗（prefab 可能為 null）</color>");
                currentData = null;
                SwitchCamera(false);
                return;
            }

            // 準備預覽（關 Collider、改 Layer 以免擋射線）
            PreparePreview(currentPreview);

            // 初始化旋轉
            currentRotation = 0f;

            if (debugLogs) Debug.Log($"<color=#ff00ff>[BuildPlacer.cs] 開始建造: <color=#00ff00>{data.prefab.name}</color></color>");
        }

        // ---------- Unity 事件：每幀更新 ----------
        private void Update()
        {
            // 快捷鍵：按 F7 開始測試建造（若沒有 preview 時）
            if (currentPreview == null && Input.GetKeyDown(KeyCode.F7))
            {
                buildingUIRoot.GetComponent<CanvasGroup>().alpha = 1; // 顯示建造按鈕
                buildingUIRoot.GetComponent<CanvasGroup>().interactable = true;
                buildingUIRoot.GetComponent<CanvasGroup>().blocksRaycasts = true;
                StartPlacing(testData);
                return;
            }

            // 若沒有在放置（沒有 preview），就跳過整個邏輯
            if (currentPreview == null) return;

            // 確保有相機可用
            Camera cam = mainCamera != null ? mainCamera : Camera.main;
            if (cam == null)
            {
                Debug.LogWarning("<color=#ff00ff>[BuildPlacer.cs] 找不到主相機（mainCamera / Camera.main 都為 null）</color>");
                return;
            }

            // 用滑鼠畫面位置發出射線到世界
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            // 只檢查 groundMask 的 Layer（Performance 好）
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask))
            {
                if (debugLogs) Debug.Log($"<color=#ff00ff>[BuildPlacer.cs] Raycast Hit: <color=#00ff00>{hit.collider.name}</color> (Layer = <color=#00ff00>{LayerMask.LayerToName(hit.collider.gameObject.layer)}</color></color>)");

                // 支援 collider 在子物件的情況，因此用 GetComponentInParent
                MapTile startTile = hit.collider.GetComponentInParent<MapTile>();
                if (startTile == null)
                {
                    // 若射到了非 MapTile 的東西，清掉高亮並跳過
                    if (debugLogs) Debug.Log("<color=#ff00ff>[BuildPlacer.cs] Raycast 打到的是非 MapTile 的物件，清除高亮</color>");
                    ResetHighlightedTiles();
                    return;
                }

                // 根據目前旋轉計算要實際佔用的 width/height（90/270 度需要 swap）
                int effW = currentData.width;
                int effH = currentData.height;
                float rotNormalized = ((currentRotation % 360) + 360) % 360;
                if (Mathf.Abs(rotNormalized - 90f) < 1f || Mathf.Abs(rotNormalized - 270f) < 1f)
                {
                    // 若接近 90 或 270 度，交換寬高
                    (effW, effH) = (effH, effW);
                }

                // 取得要佔用的所有格子（以 startTile 為左上角 anchor）
                List<MapTile> occupiedTiles = GetTilesToOccupy(startTile.row, startTile.col, effW, effH);

                // 檢查是否所有格子都能放（數量與都未被佔用）
                bool canPlace = occupiedTiles.Count == effW * effH &&
                                occupiedTiles.TrueForAll(t => !t.IsOccupied && t.transform.childCount == 0);

                if (debugLogs) Debug.Log($"<color=#ff00ff>[BuildPlacer.cs] startTile = <color=#00ff00>({startTile.row},{startTile.col})</color> <color=#00ff00>effW={effW} effH={effH} occupiedCount={occupiedTiles.Count} canPlace={canPlace}</color></color>");

                // 把預覽放在所有佔用格的中心（讓多格建築置中）
                if (occupiedTiles.Count > 0)
                {
                    Vector3 center = Vector3.zero;
                    foreach (var t in occupiedTiles) center += t.transform.position;
                    center /= occupiedTiles.Count;
                    currentPreview.transform.position = center + Vector3.up * 0.5f; // 稍微抬高一點以避免 Z-fighting
                    currentPreview.transform.rotation = Quaternion.Euler(0, currentRotation, 0);
                }
                else
                {
                    // 若沒有取得格子（超出地圖邊界），就放在被擊中的那一格
                    currentPreview.transform.position = startTile.transform.position + Vector3.up * 0.5f;
                    currentPreview.transform.rotation = Quaternion.Euler(0, currentRotation, 0);
                }

                // 高亮格子（綠/紅），並改預覽顏色（使用 overlay 色）
                HighlightTiles(occupiedTiles, canPlace);
                SetPreviewColor(canPlace ? canPlaceOverlay : cannotPlaceOverlay);

                // 左鍵放置
                if (Input.GetMouseButtonDown(0) && canPlace)
                {
                    PlaceBuilding(currentData, occupiedTiles, currentRotation);
                    EndPlacing(); // 放完就結束建造模式
                }
            }
            else
            {
                // 射線沒打到任何地面：清除高亮
                if (debugLogs) Debug.Log("<color=#ff00ff>[BuildPlacer.cs] Raycast 未命中 groundMask</color>");
                ResetHighlightedTiles();
            }

            // 鼠標滾輪改旋轉（滾一次 90 度，按 R 時 15 度）
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.0001f)
            {
                float step = Input.GetKey(KeyCode.R) ? 15f : 90f;
                currentRotation += scroll > 0 ? step : -step;
                if (debugLogs) Debug.Log($"<color=#ff00ff>[BuildPlacer.cs] currentRotation 變更為 <color=#00ff00>{currentRotation}</color></color>");
            }

            // 右鍵取消建造
            if (Input.GetMouseButtonDown(1)) EndPlacing();
        }

        // ---------- 結束建造模式並清理 ----------
        private void EndPlacing()
        {
            //if (currentPreview != null) Destroy(currentPreview); // 刪掉預覽物件
            currentPreview = null;
            currentData = null;
            currentRotation = 0f;
            ResetHighlightedTiles();
            SwitchCamera(false); // 還原相機與 UI
            if (debugLogs) Debug.Log("<color=#ff00ff>[BuildPlacer.cs] EndPlacing 執行，回到遊戲模式</color>");
        }

        // ---------- 正式建造（建立實體建築） ----------
        public void PlaceBuilding(BuildingData data, List<MapTile> tiles, float rotationY)
        {
            // 位置也用 tiles 的中心（和預覽保持一致）
            Vector3 center = Vector3.zero;
            foreach (var t in tiles) center += t.transform.position;
            center /= tiles.Count;
            Vector3 position = center + Vector3.up * 0.5f;
            Quaternion rotation = Quaternion.Euler(0, rotationY, 0);

            // 建立正式建築物
            GameObject building = Instantiate(data.prefab, position, rotation);

            // 確保 Building component 存在並初始化佔用格子
            Building buildingComponent = building.GetComponent<Building>() ?? building.AddComponent<Building>();
            buildingComponent.Init(data, tiles.ToArray());

            // 標記每個格子為已佔用並記錄建築
            foreach (var tile in tiles)
            {
                tile.IsOccupied = true;
                tile.CurrentBuilding = building;
            }

            if (debugLogs) Debug.Log($"<color=#ff00ff>[BuildPlacer.cs] 已放置建築：<color=#00ff00>{data.GetDisplayName()}</color> at center <color=#00ff00>{position}</color></color>");
        }

        // ---------- 取得要佔用的格子（左上角 anchor） ----------
        private List<MapTile> GetTilesToOccupy(int startRow, int startCol, int width, int height)
        {
            List<MapTile> result = new List<MapTile>();

            // 注意：若地圖格子很多，FindObjectsOfType 會較慢；此處簡單直接
            foreach (MapTile tile in FindObjectsOfType<MapTile>())
            {
                if (tile.row >= startRow && tile.row < startRow + height &&
                    tile.col >= startCol && tile.col < startCol + width)
                {
                    result.Add(tile);
                }
            }

            // 排序成由上（row）到左（col）順序，方便閱讀或後續處理
            result.Sort((a, b) =>
            {
                int r = a.row.CompareTo(b.row);
                if (r != 0) return r;
                return a.col.CompareTo(b.col);
            });

            return result;
        }

        // ---------- 將 preview 設為不影響射線（關 Collider 並改 Layer） ----------
        private void PreparePreview(GameObject obj)
        {
            if (obj == null) return;

            // 關閉 collider，避免預覽本身擋住射線
            var colliders = obj.GetComponentsInChildren<Collider>();
            foreach (var c in colliders) c.enabled = false;

            // 把整棵預覽物件改 layer 為 Ignore Raycast，避免 Physics.Raycast 打到預覽
            SetLayerRecursively(obj.transform, LayerMask.NameToLayer("Ignore Raycast"));

            if (debugLogs) Debug.Log("<color=#ff00ff>[BuildPlacer.cs] PreparePreview 已完成 (Collider disabled, Layer set to Ignore Raycast)</color>");
        }

        // ---------- 工具：遞迴設定 layer ----------
        private void SetLayerRecursively(Transform t, int layer)
        {
            t.gameObject.layer = layer;                // 設當前物件的 layer
            foreach (Transform c in t) SetLayerRecursively(c, layer); // 遞迴子物件
        }

        // ---------- 設定預覽顏色（疊色，但保留貼圖細節） ----------
        private void SetPreviewColor(Color overlay)
        {
            // 第一個防護：如果沒有 preview 就直接回
            if (currentPreview == null)
            {
                if (debugLogs) Debug.LogWarning("<color=#ff00ff>[BuildPlacer.cs] SetPreviewColor 被呼叫，但 currentPreview 為 null</color>");
                return;
            }

            if (debugLogs) Debug.Log($"<color=#ff00ff>[BuildPlacer.cs] SetPreviewColor called with overlay = <color=#00ff00>{overlay}</color></color>");

            // 走訪所有 Renderer，改材質的 color（改 material 而非 sharedMaterial，避免污染 prefab）
            foreach (var renderer in currentPreview.GetComponentsInChildren<Renderer>())
            {
                // 先取得材質陣列（會回傳 instance copies）
                Material[] mats = renderer.materials;
                for (int i = 0; i < mats.Length; i++)
                {
                    Material mat = mats[i];
                    if (mat == null) continue;

                    // 保留原始顏色
                    Color baseColor = mat.color;

                    // 比較保守的疊色方式：依 overlay alpha 做 Lerp（較能保留貼圖與明暗）
                    Color blended = Color.Lerp(baseColor, overlay, overlay.a);

                    // 設定 alpha：如果 overlay alpha 為 0，採用 baseColor 的 alpha；否則採 overlay 的 alpha（讓預覽半透明）
                    float finalAlpha = overlay.a > 0f ? overlay.a : baseColor.a;
                    blended.a = finalAlpha;

                    // 套回材質
                    mats[i].color = blended;

                    // 若材質支援 URP 的 _Surface 屬性（0=opaque,1=transparent），則確保透明模式
                    if (mat.HasProperty("_Surface")) mat.SetFloat("_Surface", 1f);
                    // 將渲染隊列設到透明區間（保險做法）
                    mat.renderQueue = 3000;
                }
                // 套回 renderer（確保多材質情況也能完整套上）
                renderer.materials = mats;
            }

            if (debugLogs) Debug.Log("<color=#ff00ff>[BuildPlacer.cs] SetPreviewColor 執行完成</color>");
        }

        // ---------- 高亮格子（呼叫 MapTile.SetColor） ----------
        private void HighlightTiles(List<MapTile> tiles, bool canPlace)
        {
            // 先還原上一批
            ResetHighlightedTiles();

            // 設定新的高亮集
            highlightedTiles = tiles ?? new List<MapTile>();

            // 使用半透明的綠/紅顏色
            Color c = canPlace ? new Color(0, 1f, 0f, 0.6f) : new Color(1f, 0f, 0f, 0.6f);
            foreach (var t in highlightedTiles)
            {
                if (t != null) t.SetColor(c);
            }
        }

        // ---------- 還原被高亮的格子顏色 ----------
        private void ResetHighlightedTiles()
        {
            if (highlightedTiles == null || highlightedTiles.Count == 0) return;
            foreach (var t in highlightedTiles)
            {
                if (t != null) t.ResetColor();
            }
            highlightedTiles.Clear();
        }

        // ---------- 切換相機與 UI 顯示 ----------
        private void SwitchCamera(bool buildMode)
        {
            if (playerCamera != null) playerCamera.gameObject.SetActive(!buildMode);
            if (buildCamera != null) buildCamera.gameObject.SetActive(buildMode);
            if (buildingUIRoot != null) buildingUIRoot.SetActive(buildMode);

            // 若場景有 Camera.main，更新 mainCamera 參考（方便 Cinemachine 切換）
            if (Camera.main != null) mainCamera = Camera.main;

            if (debugLogs) Debug.Log($"<color=#ff00ff>[BuildPlacer] SwitchCamera buildMode = <color=#00ff00>{buildMode}</color></color>");
        }
    }
}
