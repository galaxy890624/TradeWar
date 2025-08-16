using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 建築管理器： <br></br>
/// - 管理所有 BuildingData 資料 <br></br>
/// - 提供查找、升級、建造功能 <br></br>
/// - 與 UI / BuildPlacer 結合 <br></br>
/// </summary>
public class BuildingManager : MonoBehaviour
{
    [Header("建築資料清單（拉入所有 BuildingData 資產）")]
    [SerializeField] private List<BuildingData> allBuildings;

    public static BuildingManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary> 根據類型與等級取得建築資料 <br></br> </summary>
    public BuildingData GetBuilding(BuildingCategory category, int level)
    {
        return allBuildings.Find(b => b.category == category && b.level == level);
    }

    /// <summary> 根據現有建築資料取得下一級 <br></br> </summary>
    public BuildingData GetNextLevel(BuildingData current)
    {
        return GetBuilding(current.category, current.level + 1);
    }

    /// <summary> 提供 UI 載入所有建築用 <br></br> </summary>
    public List<BuildingData> GetAllBuildings() => allBuildings;

    // ======================
    // UI 互動：按鈕點擊後啟動放置
    // ======================
    /// <summary>
    /// UI 呼叫：選擇某建築並進入放置模式 <br></br>
    /// </summary>
    public void SelectBuilding(BuildingCategory category, int level)
    {
        var data = GetBuilding(category, level);
        if (data == null)
        {
            Debug.LogError($"[BuildingManager] 找不到 {category} Lv{level} 建築資料！");
            return;
        }

        // 呼叫你現有的 BuildPlacer 進入建造模式（在 galaxy890624 命名空間內）
        galaxy890624.BuildPlacer.Instance.StartPlacing(data);
    }

    // （可選）若你還有其他地方用到，保留原本 TryBuild 介面
    public bool TryBuild(MapTile tile, BuildingCategory category, int level)
    {
        var building = GetBuilding(category, level);
        if (building == null) return false;

        // 這裡讓 BuildPlacer 統一處理比較一致；如果你有 MapTile.Build() 也可改回去
        if (tile == null || tile.IsOccupied) return false;

        // 單格建造（多格請仍由 BuildPlacer 控）
        var go = Instantiate(building.prefab, tile.transform.position + Vector3.up * 0.5f, Quaternion.identity);
        tile.IsOccupied = true;
        tile.CurrentBuilding = go;
        return true;
    }
}