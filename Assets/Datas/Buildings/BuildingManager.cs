using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 統一管理所有 BuildingData 資料
/// 提供查找與建造介面：
/// GetBuilding() → 根據分類與等級找到資料
/// GetNextLevel() → 找到升級對象
/// TryBuild() → 嘗試在某塊地格建造建築
/// GetAllBuildings() → 取得所有建築資料（提供 UI 使用）
/// </summary>
public class BuildingManager : MonoBehaviour
{
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

    /// <summary>
    /// 根據類型與等級取得建築資料
    /// </summary>
    public BuildingData GetBuilding(BuildingCategory category, int level)
    {
        return allBuildings.Find(b => b.category == category && b.level == level);
    }

    /// <summary>
    /// 根據現有建築資料取得下一級
    /// </summary>
    public BuildingData GetNextLevel(BuildingData current)
    {
        return GetBuilding(current.category, current.level + 1);
    }

    /// <summary>
    /// 嘗試建造建築在指定地格上
    /// </summary>
    public bool TryBuild(MapTile tile, BuildingCategory category, int level)
    {
        var building = GetBuilding(category, level);
        if (building == null) return false;
        return tile.Build(building, 0f); // 旋轉暫時用 0f, 未來希望可以加入使用者自行旋轉功能
    }

    /// <summary>
    /// 提供給 UI 載入所有建築用
    /// </summary>
    public List<BuildingData> GetAllBuildings()
    {
        return allBuildings;
    }
}
