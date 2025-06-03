using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 在場景設計階段就生成地圖物件（支援置中排列與隨機一棵樹）
/// </summary>
[ExecuteInEditMode]
public class MapManager : MonoBehaviour
{
    [Header("地圖格子設定")]
    [SerializeField] private GameObject MapPrefab;
    [SerializeField, Range(1, 1000)] private int MapRowCount = 3;
    [SerializeField, Range(1, 1000)] private int MapColumnCount = 4;
    [SerializeField, Range(1f, 40f)] private float MapSpacing = 10.0f;

    [Header("樹木設定")]
    [SerializeField] private List<GameObject> TreePrefabs = new();
    [SerializeField, Range(0f, 1f)] private float TreeSpawnProbability = 0.3f;

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
                if (tile.GetComponent<MapTile>() == null)
                {
                    tile.AddComponent<MapTile>();
                }

                // 每格最多放一棵置中的樹
                if (TreePrefabs.Count > 0 && Random.value < TreeSpawnProbability)
                {
                    GameObject treePrefab = TreePrefabs[Random.Range(0, TreePrefabs.Count)];
                    Vector3 treePos = position;
                    GameObject tree = Instantiate(treePrefab, treePos, Quaternion.identity, tile.transform);
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
