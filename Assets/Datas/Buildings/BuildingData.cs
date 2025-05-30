using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 建築的科技樹
/// 比如說 : 
/// 一般房屋 1 -> 2 -> 3 -> ... 等級
/// 交易所 1 -> 2 -> ... 等級
/// </summary>
[CreateAssetMenu(fileName = "BuildingData", menuName = "Building Data/New Building Data")]

public class BuildingData : ScriptableObject
{
    [Header("建築名稱")]
    [SerializeField]
    private string BuildingName;
    [Header("建築的預製物")]
    [SerializeField]
    private GameObject BuildingPrefab;
    [Header("建築等級")]
    [SerializeField]
    private int BuildingLevel;
    [Header("生成在地圖塊的第幾列")]
    [SerializeField]
    private int RowIndex;
    [Header("生成在地圖塊的第幾行")]
    [SerializeField]
    private int ColumnIndex;
    [Header("建築的方向")]
    [SerializeField]
    private Vector3 BuildingDirection = Vector3.zero;
    [Header("消耗金錢")]
    [SerializeField]
    private int CostMoney;
    [Header("消耗黃金")]
    [SerializeField]
    private int CostGold;
    [Header("消耗木材")]
    [SerializeField]
    private int CostWood;
}
