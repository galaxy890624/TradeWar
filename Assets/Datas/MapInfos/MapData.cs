using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 地圖資料 <br><br></br></br>
/// 記錄每一個地圖塊包含的資料
/// </summary>
[CreateAssetMenu(fileName = "MapData", menuName = "Map Data/New Map Data")]
public class MapData : ScriptableObject
{
    [Header("Map Tile Rows")]
    [SerializeField, Range(0, 1000)] public int Rows;
    [Header("Map Tile Cols")]
    [SerializeField, Range(0, 1000)] public int Cols;
    [Header("地圖塊物件的間隔")]
    [SerializeField, Range(0f, 20f)] public float MapSpacing = 10.0f;
    [Header("地圖物件列表")]
    [SerializeField] public GameObject[] MapPrefab;

    [Header("地圖物件配置 (2D Array模擬)")]
    [SerializeField] public GameObject[] MapInfo;

    /// <summary>
    /// 獲取指定位置的物件 <br></br>
    /// </summary>
    /// <param name="row">行索引 (0-based) <br></br></param>
    /// <param name="col">列索引 (0-based) <br></br></param>
    /// <returns>該位置的GameObject，如果為空則返回null <br></br></returns>
    public GameObject GetTileObject(int row, int col)
    {
        if (!IsValidPosition(row, col))
        {
            Debug.LogError($"Invalid position: ({row}, {col}). Map size is {Rows}x{Cols}");
            return null;
        }

        int index = row * Cols + col;
        return MapInfo[index];
    }

    /// <summary>
    /// 設置指定位置的物件 <br></br>
    /// </summary>
    /// <param name="row">行索引 (0-based) <br></br></param>
    /// <param name="col">列索引 (0-based) <br></br></param>
    /// <param name="gameObject">要放置的GameObject <br></br></param>
    /// <returns>是否設置成功 <br></br></returns>
    public bool SetTileObject(int row, int col, GameObject gameObject)
    {
        if (!IsValidPosition(row, col))
        {
            Debug.LogError($"Invalid position: ({row}, {col}). Map size is {Rows}x{Cols}");
            return false;
        }

        int index = row * Cols + col;

        // 檢查該位置是否已經有物件
        if (MapInfo[index] != null && gameObject != null)
        {
            Debug.LogWarning($"Position ({row}, {col}) already has an object. Replacing it.");
        }

        MapInfo[index] = gameObject;
        return true;
    }

    /// <summary>
    /// 檢查指定位置是否為空
    /// </summary>
    /// <param name="row">行索引</param>
    /// <param name="col">列索引</param>
    /// <returns>true表示該位置為空</returns>
    public bool IsTileEmpty(int row, int col)
    {
        if (!IsValidPosition(row, col))
            return false;

        int index = row * Cols + col;
        return MapInfo[index] == null;
    }

    /// <summary>
    /// 清空指定位置的物件
    /// </summary>
    /// <param name="row">行索引</param>
    /// <param name="col">列索引</param>
    public void ClearTile(int row, int col)
    {
        if (IsValidPosition(row, col))
        {
            int index = row * Cols + col;
            MapInfo[index] = null;
        }
    }

    /// <summary>
    /// 檢查座標是否有效
    /// </summary>
    /// <param name="row">行索引</param>
    /// <param name="col">列索引</param>
    /// <returns>座標是否在有效範圍內</returns>
    public bool IsValidPosition(int row, int col)
    {
        return row >= 0 && row < Rows && col >= 0 && col < Cols;
    }

    /// <summary>
    /// 初始化地圖陣列
    /// 當Rows或Cols改變時需要調用此方法
    /// </summary>
    [ContextMenu("Initialize Map Array")]
    public void InitializeMapArray()
    {
        int totalSize = Rows * Cols;
        if (MapInfo == null || MapInfo.Length != totalSize)
        {
            GameObject[] oldMapInfo = MapInfo;
            MapInfo = new GameObject[totalSize];

            // 如果有舊資料，盡量保留
            if (oldMapInfo != null)
            {
                int copyLength = Mathf.Min(oldMapInfo.Length, totalSize);
                for (int i = 0; i < copyLength; i++)
                {
                    MapInfo[i] = oldMapInfo[i];
                }
            }
        }
    }

    /// <summary>
    /// 清空整個地圖
    /// </summary>
    [ContextMenu("Clear All Tiles")]
    public void ClearAllTiles()
    {
        if (MapInfo != null)
        {
            for (int i = 0; i < MapInfo.Length; i++)
            {
                MapInfo[i] = null;
            }
        }
    }

    /// <summary>
    /// 獲取地圖的總格子數
    /// </summary>
    public int TotalTiles => Rows * Cols;

    // Unity Editor專用：當Inspector中的值改變時自動調用
    private void OnValidate()
    {
        // 確保陣列大小與地圖尺寸匹配
        if (Rows > 0 && Cols > 0)
        {
            InitializeMapArray();
        }
    }
}