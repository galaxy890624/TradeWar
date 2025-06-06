using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// 附加在實體建築上，用來記錄這個建築屬於哪一筆 BuildingData <br></br>
/// 可用來做動畫、升級邏輯、產出等擴充行為
/// </summary>
public class Building : MonoBehaviour
{
    // 建築資料
    public BuildingData data;
    // 被佔用的所有格子
    private MapTile[] occupiedTiles;

    /// <summary>
    /// 初始化建築與佔用格子
    /// </summary>
    public void Init(BuildingData buildingData, MapTile[] tiles)
    {
        data = buildingData;
        occupiedTiles = tiles;

        // 建築名稱顯示
        gameObject.name = data.GetDisplayName();
    }

    void OnMouseDown()
    {
        // 點擊建築時 → 顯示升級 UI
        BuildingUpgradeUI.Instance.Show(this);
    }

    /// <summary>
    /// 升級建築（由 UI 呼叫）
    /// </summary>
    public void Upgrade()
    {
        // 取得下一級資料
        var nextLevel = BuildingManager.Instance.GetNextLevel(data);
        if (nextLevel == null)
        {
            Debug.Log("已是最高等級！");
            return;
        }

        // 移除舊建築
        foreach (var tile in occupiedTiles)
        {
            tile.IsOccupied = false;
            tile.CurrentBuilding = null;
        }

        Destroy(gameObject);

        // 呼叫 BuildPlacer 建造新建築在原格
        BuildPlacer.Instance.PlaceBuilding(nextLevel, occupiedTiles.ToList());
    }

    /// <summary>
    /// 拆除建築（由 UI 呼叫）
    /// </summary>
    public void Demolish()
    {
        foreach (var tile in occupiedTiles)
        {
            tile.IsOccupied = false;
            tile.CurrentBuilding = null;
        }

        Destroy(gameObject);
        BuildingUpgradeUI.Instance.Hide();
    }
    /// <summary>
    /// Gets the smallest row index among the occupied tiles.
    /// </summary>
    /// <remarks>This method iterates through all occupied tiles to determine the smallest row
    /// index.</remarks>
    /// <returns>The minimum row index of the occupied tiles. If no tiles are occupied, returns <see cref="int.MaxValue"/>.</returns>
    public int GetLeftTopRow()
    {
        int min = int.MaxValue;
        foreach (var t in occupiedTiles)
            min = Mathf.Min(min, t.row);
        return min;
    }
    /// <summary>
    /// Gets the smallest column index among the occupied tiles.
    /// </summary>
    /// <remarks>This method iterates through the collection of occupied tiles to determine the smallest
    /// column index.</remarks>
    /// <returns>The minimum column index of the occupied tiles. If no tiles are occupied, returns <see cref="int.MaxValue"/>.</returns>
    public int GetLeftTopCol()
    {
        int min = int.MaxValue;
        foreach (var t in occupiedTiles)
            min = Mathf.Min(min, t.col);
        return min;
    }
}
