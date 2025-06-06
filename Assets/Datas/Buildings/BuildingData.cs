using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 儲存一筆建築的完整資料（分類、等級、成本、Prefab） <br></br>
/// 每一筆資料代表一個具體建築，如「House Lv1」、「Market Lv2」 <br></br><br></br>
/// 建築的科技樹 <br></br>
/// 比如說 :  <br></br>
/// 一般房屋 1 -> 2 -> 3 -> ... 等級 <br></br>
/// 交易所 1 -> 2 -> ... 等級 <br></br>
/// </summary>
[CreateAssetMenu(fileName = "BuildingData", menuName = "Building Data/New Building Data")]

public class BuildingData : ScriptableObject
{
    [Header("建築分類與等級")]
    public BuildingCategory category;
    public int level;

    [Header("建築基本資料")]
    public string displayName;
    public GameObject prefab;
    public Sprite icon;

    [Header("建造成本")]
    public int costMoney;
    public int costWood;
    public int costGold;

    // 建築可佔多格（預設為1x1）
    [Header("建築佔用空間")]
    public int width = 1;  // 建築橫向佔用格數
    public int height = 1; // 建築縱向佔用格數

    public string GetDisplayName()
    {
        return string.IsNullOrEmpty(displayName)
            ? $"<color=#ff00ff><color=#00ff00>{category}</color> Lv.<color=#00ff00>{level}</color></color>"
            : displayName;
    }
}