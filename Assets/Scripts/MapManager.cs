using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 在場景設計階段就生成地圖物件
/// </summary>
[ExecuteInEditMode]
public class MapManager : MonoBehaviour
{
    [Header("自動生成地圖物件")]
    [SerializeField] private GameObject MapPrefab;
    [Header("地圖物件數量")]
    [SerializeField, Range(0, 200)] private int MapRowCount = 3;
    [SerializeField, Range(0, 200)] private int MapColumnCount = 4;
    [Header("地圖物件間距")]
    [SerializeField, Range(0f, 40f)] private float MapSpacing = 10.0f;

    private void OnValidate()
    {
        Debug.Log("<color=#ff00ff>[MapManager] OnValidate 被執行</color>");

        if (!Application.isPlaying)
        {
            ClearMap();  // 清空原有地圖
            GenerateMap(); // 重新生成
        }
    }

    private void GenerateMap()
    {
        if (MapPrefab == null)
        {
            Debug.LogWarning("MapPrefab 未設定，請拖入一個預製件！");
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

                Debug.Log($"<color=#ff00ff>生成 Tile：</color><color=#00ff00>({i}, {j}) → {position}</color>");
            }
        }
    }

    private void ClearMap()
    {
        // 清除之前產生的子物件
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}