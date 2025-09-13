using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 編輯器模式地圖生成器
/// </summary>
[ExecuteInEditMode]
public class MapManager : MonoBehaviour
{
    [Header("地圖格子設定")]
    [SerializeField] private GameObject MapPrefab; // 預設地面格
    [SerializeField, Range(1, 1000)] public int MapRowCount = 3;
    [SerializeField, Range(1, 1000)] public int MapColumnCount = 4;
    [SerializeField, Range(1f, 40f)] public float MapSpacing = 10.0f;

    [Header("地圖塊上的物件資料來源")]
    [SerializeField] public MapData MapData;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying) return;

        // 已經有 MapTile 就跳過生成
        if (HasTiles())
        {
            Debug.Log("<color=#ff00ff>[MapManager.cs] 已有地圖，跳過自動生成</color>");
            return;
        }

        GenerateMap();
    }
#endif

    /// <summary>
    /// 檢查 MapManager 底下是否已經有地圖格子
    /// </summary>
    private bool HasTiles()
    {
        foreach (Transform child in transform)
        {
            if (child.GetComponent<MapTile>() != null)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 生成地圖
    /// </summary>
    public void GenerateMap()
    {
        if (MapPrefab == null)
        {
            Debug.LogWarning("[MapManager] MapPrefab 未指定！");
            return;
        }

        // 先清空
        ClearMap();

        float offsetX = (MapColumnCount - 1) * MapSpacing / 2f;
        float offsetZ = (MapRowCount - 1) * MapSpacing / 2f;

        for (int i = 0; i < MapRowCount; i++)
        {
            for (int j = 0; j < MapColumnCount; j++)
            {
                Vector3 position = new Vector3(j * MapSpacing - offsetX, 0, i * MapSpacing - offsetZ);
                GameObject tile = Instantiate(MapPrefab, position, Quaternion.identity, transform);
                tile.name = $"Tile_{i}_{j}";

                // 確保有 MapTile 腳本
                MapTile tileScript = tile.GetComponent<MapTile>();
                if (tileScript == null)
                    tileScript = tile.AddComponent<MapTile>();

                tileScript.Init(i, j);

                // 設定 Layer
                tile.layer = LayerMask.NameToLayer("Ground");

                // 為了不讓地圖格子穿透滑鼠射線，確保有 Collider + Layer = Ground
                if (tile.GetComponent<Collider>() == null) tile.gameObject.AddComponent<BoxCollider>();

                // 從 MapData 讀取預設物件
                GameObject prefab = MapData != null ? MapData.GetTileObject(i, j) : null;
                if (prefab != null)
                {
                    GameObject content = Instantiate(prefab, position, Quaternion.identity, tile.transform);
                    content.name = prefab.name;
                }

                // 設定佔用狀態
                tileScript.IsOccupied = tile.transform.childCount > 0;
            }
        }

        Debug.Log("<color=green>[MapManager] 地圖生成完成！</color>");
    }

    /// <summary>
    /// 清空地圖
    /// </summary>
    public void ClearMap()
    {
#if UNITY_EDITOR
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
#endif
    }

#if UNITY_EDITOR
    // 在 Inspector 中加按鈕
    [CustomEditor(typeof(MapManager))]
    public class MapManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            MapManager manager = (MapManager)target;
            
            GUILayout.Space(10); // 先空一段 10 像素的距離（讓按鈕和前面的欄位分開，不會貼在一起），再畫出「生成地圖」按鈕。如果沒有 GUILayout.Space(10);，按鈕會直接貼在上方的 Inspector 欄位，看起來比較擁擠。
            if (GUILayout.Button("生成地圖"))
            {
                manager.GenerateMap();
            }
            if (GUILayout.Button("清空地圖"))
            {
                manager.ClearMap();
            }
        }
    }
#endif
}
