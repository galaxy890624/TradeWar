using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 儲存與載入所有建築狀態
/// </summary>
public class SaveSystem : MonoBehaviour
{
    public string saveFileName = "save.json";

    private string FullPath => Path.Combine(Application.persistentDataPath, saveFileName);

    /// <summary>
    /// 儲存所有建築資料
    /// </summary>
    public void Save()
    {
        var allBuildings = FindObjectsOfType<Building>();
        SaveData save = new();

        foreach (var b in allBuildings)
        {
            BuildingSaveData data = new()
            {
                category = b.data.category,
                level = b.data.level,
                rotationY = b.transform.rotation.eulerAngles.y,
                row = b.GetLeftTopRow(),
                col = b.GetLeftTopCol()
            };

            save.buildings.Add(data);
        }

        string json = JsonUtility.ToJson(save, true);
        File.WriteAllText(FullPath, json);
        Debug.Log($"存檔完成：{FullPath}");
    }

    /// <summary>
    /// 載入建築資料並重建地圖
    /// </summary>
    public void Load()
    {
        if (!File.Exists(FullPath))
        {
            Debug.LogWarning("找不到存檔");
            return;
        }

        string json = File.ReadAllText(FullPath);
        SaveData save = JsonUtility.FromJson<SaveData>(json);

        foreach (var b in FindObjectsOfType<Building>())
        {
            Destroy(b.gameObject);
        }

        foreach (var tile in FindObjectsOfType<MapTile>())
        {
            tile.IsOccupied = false;
            tile.CurrentBuilding = null;
        }

        foreach (var entry in save.buildings)
        {
            var data = BuildingManager.Instance.GetBuilding(entry.category, entry.level);
            var tiles = GetTiles(entry.row, entry.col, data.width, data.height);
            BuildPlacer.Instance.PlaceBuilding(data, tiles, entry.rotationY);
        }

        Debug.Log("讀檔完成");
    }

    /// <summary>
    /// 從 row/col 找出多格 tile 區塊
    /// </summary>
    private List<MapTile> GetTiles(int startRow, int startCol, int w, int h)
    {
        List<MapTile> list = new();

        foreach (var tile in FindObjectsOfType<MapTile>())
        {
            if (tile.row >= startRow && tile.row < startRow + h &&
                tile.col >= startCol && tile.col < startCol + w)
            {
                list.Add(tile);
            }
        }

        return list;
    }
}