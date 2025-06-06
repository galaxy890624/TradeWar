using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 放置控制器 <br></br><br></br>
/// 控制放置流程（預覽、旋轉、確認建造）<br></br>
/// </summary>
public class BuildPlacer : MonoBehaviour
{
    public static BuildPlacer Instance { get; private set; }

    public LayerMask groundMask;
    public Material previewMaterial;

    private GameObject currentPreview;
    private BuildingData currentData;
    private float currentRotation = 0f;

    private void Awake()
    {
        Instance = this;
    }
    /// <summary>
    /// 點擊建築選單按鈕 → 開始建築預覽模式
    /// </summary>
    public void StartPlacing(BuildingData data)
    {
        currentData = data;

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

                Destroy(currentPreview);
                currentPreview = null;
                currentData = null;
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
        if (Input.GetMouseButtonDown(1))
        {
            Destroy(currentPreview);
            currentPreview = null;
            currentData = null;
        }
    }

    /// <summary>
    /// 建立建築物在指定的 tiles 區域
    /// 可供升級、載入、編輯器產生等用途
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
    /// 找出從左上角開始，寬×高 的所有格子
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
    /// 將預覽建築套用透明材質
    /// </summary>
    private void ApplyPreviewMaterial(GameObject obj)
    {
        foreach (var renderer in obj.GetComponentsInChildren<Renderer>())
        {
            renderer.material = previewMaterial;
        }
    }
}

// 建議搭配的 Unity UI 結構：
// - Canvas
//   - Panel (靠右固定定位，Vertical Layout Group)
//     - ButtonPrefab (含 icon + name + 建立事件)
// - 所有格子有 Collider + MapTile.cs，需加 tag/layer 給 Raycast
