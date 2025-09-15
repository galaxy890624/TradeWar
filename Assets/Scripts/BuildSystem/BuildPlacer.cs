using Cinemachine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace galaxy890624
{
    /// <summary>
    /// 改良版 BuildPlacer
    /// - 避免 preview 擋射線
    /// - 處理旋轉(90/270)交換 width/height
    /// - 高亮要佔用的格子 (綠/紅)
    /// - 對齊到選取格子中心
    /// </summary>
    public class BuildPlacer : MonoBehaviour
    {
        public static BuildPlacer Instance { get; private set; }

        [Header("建築放置設定")]
        public LayerMask groundMask;
        //public Material previewMaterial;

        [Header("相機與 UI 控制")]
        public CinemachineVirtualCamera buildCamera;
        public CinemachineVirtualCamera playerCamera;
        public GameObject buildingUIRoot;
        [Tooltip("若場景中有多個 Camera，可手動指定；否則會使用 Camera.main")]
        public Camera mainCamera;

        [Header("測試用建築資料 (F7 啟動)")]
        public BuildingData testData;

        private GameObject currentPreview;
        private BuildingData currentData;
        private float currentRotation;

        // 目前被高亮的 tiles（用於重置顏色）
        private List<MapTile> highlightedTiles = new();

        private void Awake()
        {
            Instance = this;
            if (buildingUIRoot != null) buildingUIRoot.SetActive(false);
            if (buildCamera != null) buildCamera.gameObject.SetActive(false);

            if (mainCamera == null) mainCamera = Camera.main;
        }

        public void StartPlacing(BuildingData data)
        {
            currentData = data;
            if (currentData == null)
            {
                Debug.LogWarning("<color=#ff00ff>[BuildPlacer.cs] 無法開始建造，資料為空</color>");
                return;
            }

            SwitchCamera(true);

            if (currentPreview != null) Destroy(currentPreview);

            // Instantiate preview
            currentPreview = Instantiate(data.prefab);
            PreparePreview(currentPreview);
            //ApplyPreviewMaterial(currentPreview);
            currentRotation = 0f;

            Debug.Log($"<color=#ff00ff>[BuildPlacer.cs] 開始建造：<color=#00ff00>{data.prefab.name}</color></color>");
        }

        void Update()
        {
            if (currentPreview == null && Input.GetKeyDown(KeyCode.F7))
            {
                StartPlacing(testData);
                return;
            }

            if (currentPreview == null) return;

            Camera cam = mainCamera != null ? mainCamera : Camera.main;
            if (cam == null)
            {
                Debug.LogWarning("<color=#00ff00>[BuildPlacer.cs] 找不到 Camera（請在 Inspector 指定 mainCamera 或場景中標記 MainCamera）</color>");
                return;
            }

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask))
            {
                // 支援 collider 在子物件或父物件的情況
                MapTile startTile = hit.collider.GetComponentInParent<MapTile>();
                if (startTile == null)
                {
                    // 有時候會打到場景上的其他物件，例如 preview；把高亮清除並跳過
                    ResetHighlightedTiles();
                    return;
                }

                // 計算旋轉後實際佔用的寬高（若旋轉為 90 或 270 度，交換 width/height）
                int effW = currentData.width;
                int effH = currentData.height;
                float rotNormalized = ((currentRotation % 360) + 360) % 360;
                if (Mathf.Abs(rotNormalized - 90f) < 1f || Mathf.Abs(rotNormalized - 270f) < 1f)
                {
                    // swap
                    (effW, effH) = (effH, effW);
                }

                List<MapTile> occupiedTiles = GetTilesToOccupy(startTile.row, startTile.col, effW, effH);

                bool canPlace = occupiedTiles.Count == effW * effH &&
                                occupiedTiles.TrueForAll(t => !t.IsOccupied && t.transform.childCount == 0);

                // 把預覽放在要佔用格的中心（如果有格子）
                if (occupiedTiles.Count > 0)
                {
                    Vector3 center = Vector3.zero;
                    foreach (var t in occupiedTiles) center += t.transform.position;
                    center /= occupiedTiles.Count;
                    currentPreview.transform.position = center + Vector3.up * 0.5f;
                    currentPreview.transform.rotation = Quaternion.Euler(0, currentRotation, 0);
                }
                else
                {
                    // fallback：放到 startTile
                    currentPreview.transform.position = startTile.transform.position + Vector3.up * 0.5f;
                    currentPreview.transform.rotation = Quaternion.Euler(0, currentRotation, 0);
                }

                // 高亮顯示
                HighlightTiles(occupiedTiles, canPlace);

                // 改變預覽顏色
                SetPreviewColor(canPlace ? Color.green : Color.red);

                if (Input.GetMouseButtonDown(0) && canPlace)
                {
                    PlaceBuilding(currentData, occupiedTiles, currentRotation);
                    EndPlacing();
                }
            }
            else
            {
                // 沒打到任何地面，清除高亮
                ResetHighlightedTiles();
            }

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.0001f)
            {
                float step = Input.GetKey(KeyCode.R) ? 15f : 90f;
                currentRotation += scroll > 0 ? step : -step;
            }

            if (Input.GetMouseButtonDown(1)) EndPlacing();
        }

        private void EndPlacing()
        {
            if (currentPreview != null) Destroy(currentPreview);
            currentPreview = null;
            currentData = null;
            currentRotation = 0f;
            ResetHighlightedTiles();
            SwitchCamera(false);
        }

        public void PlaceBuilding(BuildingData data, List<MapTile> tiles, float rotationY)
        {
            // 位置放到 tiles 的中心（一致於 preview）
            Vector3 center = Vector3.zero;
            foreach (var t in tiles) center += t.transform.position;
            center /= tiles.Count;
            Vector3 position = center + Vector3.up * 0.5f;
            Quaternion rotation = Quaternion.Euler(0, rotationY, 0);

            GameObject building = Instantiate(data.prefab, position, rotation);
            // 若 prefab 已經帶 Building，勿重複 Add
            Building buildingComponent = building.GetComponent<Building>() ?? building.AddComponent<Building>();
            buildingComponent.Init(data, tiles.ToArray());

            foreach (var tile in tiles)
            {
                tile.IsOccupied = true;
                tile.CurrentBuilding = building;
            }
        }

        private List<MapTile> GetTilesToOccupy(int startRow, int startCol, int width, int height)
        {
            List<MapTile> result = new();
            // 先把所有 tiles 全抓出來（如果場景格子很多，建議改成 MapManager 提供快速查表）
            foreach (MapTile tile in FindObjectsOfType<MapTile>())
            {
                if (tile.row >= startRow && tile.row < startRow + height &&
                    tile.col >= startCol && tile.col < startCol + width)
                {
                    result.Add(tile);
                }
            }

            // 確保 result 的排序（由 row, col 排序），方便後續處理（可選）
            result.Sort((a, b) =>
            {
                int r = a.row.CompareTo(b.row);
                if (r != 0) return r;
                return a.col.CompareTo(b.col);
            });

            return result;
        }

        private void PreparePreview(GameObject obj)
        {
            // 禁用 Collider（避免擋射線），並把 layer 設為 Ignore Raycast（可選）
            var colliders = obj.GetComponentsInChildren<Collider>();
            foreach (var c in colliders) c.enabled = false;

            // 設所有子物件 layer 為 IgnoreRaycast (讓 Physics.Raycast 不會擊中)
            SetLayerRecursively(obj.transform, LayerMask.NameToLayer("Ignore Raycast"));
        }

        private void SetLayerRecursively(Transform t, int layer)
        {
            t.gameObject.layer = layer;
            foreach (Transform c in t) SetLayerRecursively(c, layer);
        }

        /*private void ApplyPreviewMaterial(GameObject obj)
        {
            foreach (var renderer in obj.GetComponentsInChildren<Renderer>())
            {
                // 使用 sharedMaterial 以避免產生大量 Material instance（視需求調整）
                renderer.material = previewMaterial;
            }
        }*/

        private void SetPreviewColor(Color color)
        {
            if (currentPreview == null) return;
            foreach (var renderer in currentPreview.GetComponentsInChildren<Renderer>())
            {
                if (renderer.material != null)
                {
                    renderer.material.color = color;
                }
            }
        }

        private void SwitchCamera(bool buildMode)
        {
            if (playerCamera != null) playerCamera.gameObject.SetActive(!buildMode);
            if (buildCamera != null) buildCamera.gameObject.SetActive(buildMode);
            if (buildingUIRoot != null) buildingUIRoot.SetActive(buildMode);

            // 更新 mainCamera（假如場景是用 Cinemachine Brain on a Camera）
            if (Camera.main != null) mainCamera = Camera.main;
        }

        private void HighlightTiles(List<MapTile> tiles, bool canPlace)
        {
            // 如果上一組與這組相同就不必重置（簡化，暫用簡單方式）
            // 清除舊的
            ResetHighlightedTiles();

            highlightedTiles = tiles;
            Color c = canPlace ? new Color(0, 1, 0, 0.6f) : new Color(1, 0, 0, 0.6f);
            foreach (var t in highlightedTiles)
            {
                t.SetColor(c);
            }
        }

        private void ResetHighlightedTiles()
        {
            if (highlightedTiles == null || highlightedTiles.Count == 0) return;
            foreach (var t in highlightedTiles)
            {
                if (t != null) t.ResetColor();
            }
            highlightedTiles.Clear();
        }
    }
}
