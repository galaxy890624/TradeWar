using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.Rendering.DebugUI.Table;

/// <summary>
/// 在場景設計階段就生成地圖物件（支援置中排列與隨機一棵樹）
/// </summary>
[ExecuteInEditMode]
public class MapManager : MonoBehaviour
{
    [Header("地圖格子設定")]
    [SerializeField] private GameObject MapPrefab; // 地圖格子預製件 (預設 : 草地)
    [SerializeField, Range(1, 1000)] public int MapRowCount = 3;
    [SerializeField, Range(1, 1000)] public int MapColumnCount = 4;
    [SerializeField, Range(1f, 40f)] public float MapSpacing = 10.0f;

    [Header("地圖塊上的物件資料來源")]
    [SerializeField] public MapData MapData;


    private void OnValidate()
    {
        Debug.Log("<color=#ff00ff>[MapManager] OnValidate 被執行</color>");

        if (!Application.isPlaying)
        {
            ClearMap();       // 清空原有地圖
            GenerateMap();    // 重新生成
        }
    }

    private void GenerateMap()
    {
        // 如果已經有子物件就不再生成
        if (transform.childCount > 0)
        {
            Debug.Log("<color=#ff00ff>[MapManager] 已有子物件, 跳過地圖生成</color>");
            return;
        }

        if (MapPrefab == null)
        {
            Debug.LogWarning("<color=#ff0000>MapPrefab 未指定, 請在 Inspector 中拖入一個預製件!</color>");
            return;
        }

        float offsetX = (MapColumnCount - 1) * MapSpacing / 2f;
        float offsetZ = (MapRowCount - 1) * MapSpacing / 2f;

        for (int i = 0; i < MapRowCount; i++)
        {
            for (int j = 0; j < MapColumnCount; j++)
            {
                Vector3 position = new Vector3(j * MapSpacing - offsetX, 0, i * MapSpacing - offsetZ);
                GameObject tile = Instantiate(MapPrefab, position, Quaternion.identity, transform);
                tile.name = $"Tile_{i}_{j}";

                // 自動加上 MapTile 腳本（如果還沒加的話）
                // 再把每一個地圖塊 圖層 指定為 Ground
                if (tile.GetComponent<MapTile>() == null)
                {
                    tile.AddComponent<MapTile>();
                    tile.layer = LayerMask.NameToLayer("Ground"); // 或 MapTile
                }

                // 設定 row/col
                tile.GetComponent<MapTile>().Init(i, j);

                // 查詢是否有設定物件 -> 放上去
                GameObject prefab = MapData.GetTileObject(i, j);
                if (prefab != null)
                {
                    GameObject content = Instantiate(prefab, position, Quaternion.identity, tile.transform);
                    content.name = $"Obj_{i}_{j}";
                }

                Debug.Log($"<color=#ff00ff>生成 Tile：</color><color=#00ff00>({i}, {j}) → {position}</color>");
            }
        }
    }

    private void ClearMap()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}
