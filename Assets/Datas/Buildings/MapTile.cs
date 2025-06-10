using UnityEngine;

/// <summary>
/// 每一個地圖格子使用的腳本 <br></br><br></br>
/// 提供建造功能 Build()： <br></br>
/// 判斷是否已被佔用 <br></br>
/// 在此格生成建築 <br></br>
/// 標記建築狀態 <br></br><br></br>
/// 可以整合點擊交互、滑鼠高亮等功能
/// </summary>
public class MapTile : MonoBehaviour
{
    [Header("格子座標（由 MapManager 設定）")]
    public int row;  // 格子的列（Z 軸方向）
    public int col;  // 格子的欄（X 軸方向）

    [Header("佔用狀態")]
    public bool IsOccupied = false;  // 是否已被建築佔用
    public GameObject CurrentBuilding = null;  // 該格上目前的建築物

    [Header("格子渲染器")]
    private Renderer rend;  // 負責改變格子的顏色

    /// <summary>
    /// 初始化格子的位置（由 MapManager 呼叫）
    /// </summary>
    public void Init(int row, int col)
    {
        this.row = row;
        this.col = col;
        gameObject.name = $"Tile_{row}_{col}";

        // 取得自身或子物件的 Renderer 用來改顏色
        rend = GetComponent<Renderer>();
        if (rend == null)
        {
            rend = GetComponentInChildren<Renderer>();
        }
    }

    /// <summary>
    /// 嘗試在此格子建造一個建築（預設為左上角格子使用）<br></br>
    /// 傳入建築資料與旋轉角度（Y 軸）<br></br>
    /// 若此格已被佔用則失敗 <br></br><br></br>
    /// 如果建築是 m × n 的多格	用 PlaceBuilding() <br></br>
    /// 如果建築是 1 × 1 的格子建築	可以用 MapTile.Build()（保留）<br></br>
    /// </summary>
    public bool Build(BuildingData data, float rotationY)
    {
        if (IsOccupied) return false;

        // 將建築放在格子上方稍微浮起
        Vector3 pos = transform.position + Vector3.up * 0.5f;
        Quaternion rot = Quaternion.Euler(0, rotationY, 0);
        GameObject building = Instantiate(data.prefab, pos, rot, transform);

        // 記錄此格已被佔用
        IsOccupied = true;
        CurrentBuilding = building;

        return true;
    }

    /// <summary>
    /// 設定格子顏色（用於建築預覽區塊）
    /// </summary>
    public void SetColor(Color color)
    {
        if (rend != null)
        {
            rend.material.color = color;
        }
    }

    /// <summary>
    /// 重置格子顏色為預設值（建議為白色）
    /// </summary>
    public void ResetColor()
    {
        SetColor(Color.white);
    }

    public void SetRiver()
    {
        // 將格子設為河流顏色（假設為藍色）
    }
}