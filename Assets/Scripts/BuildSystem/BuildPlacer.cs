using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

namespace galaxy890624
{
    /// <summary>
    /// 建築放置控制器 <br></br>
    /// </summary>
    public class BuildPlacer : MonoBehaviour
    {
        public static BuildPlacer Instance { get; private set; }

        [Header("建築放置設定")]
        public LayerMask groundMask;
        public Material previewMaterial;

        [Header("相機與 UI 控制")]
        public CinemachineVirtualCamera buildCamera;
        public CinemachineVirtualCamera playerCamera;
        public GameObject buildingUIRoot;

        [Header("測試用建築資料 (F7 啟動)")]
        public BuildingData testData;

        private GameObject currentPreview;
        private BuildingData currentData;
        private float currentRotation;

        private void Awake()
        {
            Instance = this;
            if (buildingUIRoot != null) buildingUIRoot.SetActive(false);
            if (buildCamera != null) buildCamera.gameObject.SetActive(false);
        }

        public void StartPlacing(BuildingData data)
        {
            currentData = data;
            if (currentData == null)
            {
                Debug.LogWarning("[BuildPlacer] 無法開始建造，資料為空");
                return;
            }

            SwitchCamera(true);
            if (currentPreview != null) Destroy(currentPreview);

            currentPreview = Instantiate(data.prefab);
            ApplyPreviewMaterial(currentPreview);
            currentRotation = 0f;

            Debug.Log($"[BuildPlacer] 開始建造：{data.prefab.name}");
        }

        void Update()
        {
            if (currentPreview == null && Input.GetKeyDown(KeyCode.F7))
            {
                StartPlacing(testData);
                return;
            }

            if (currentPreview == null) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.Log($"<color=#ff00ff>[BuildPlacer.cs] ray = <color=#00ff00>{ray}</color></color>");
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask))
            {
                MapTile startTile = hit.collider.GetComponent<MapTile>();
                if (startTile == null) return;

                List<MapTile> occupiedTiles = GetTilesToOccupy(startTile.row, startTile.col, currentData.width, currentData.height);

                bool canPlace = occupiedTiles.Count == currentData.width * currentData.height &&
                                occupiedTiles.TrueForAll(t => !t.IsOccupied && t.transform.childCount == 0);

                currentPreview.transform.position = startTile.transform.position + Vector3.up * 0.5f;
                currentPreview.transform.rotation = Quaternion.Euler(0, currentRotation, 0);
                SetPreviewColor(canPlace ? Color.green : Color.red);

                if (Input.GetMouseButtonDown(0) && canPlace)
                {
                    PlaceBuilding(currentData, occupiedTiles, currentRotation);
                    EndPlacing();
                }
            }

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
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
            SwitchCamera(false);
        }

        public void PlaceBuilding(BuildingData data, List<MapTile> tiles, float rotationY)
        {
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

        private void ApplyPreviewMaterial(GameObject obj)
        {
            foreach (var renderer in obj.GetComponentsInChildren<Renderer>())
            {
                renderer.material = previewMaterial;
            }
        }

        private void SetPreviewColor(Color color)
        {
            foreach (var renderer in currentPreview.GetComponentsInChildren<Renderer>())
            {
                renderer.material.color = color;
            }
        }

        private void SwitchCamera(bool buildMode)
        {
            if (playerCamera != null) playerCamera.gameObject.SetActive(!buildMode);
            if (buildCamera != null) buildCamera.gameObject.SetActive(buildMode);
            if (buildingUIRoot != null) buildingUIRoot.SetActive(buildMode);
        }
    }
}
