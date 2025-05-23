using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家資料
/// fillName 新創建的檔案名稱
/// menuName 右鍵的目錄名稱
/// </summary>
[CreateAssetMenu(fileName = "PlayerData", menuName = "Player Data/New Player Data")]
public class PlayerData : ScriptableObject
{
    /// <summary>
    /// 持倉情況
    /// </summary>
    [Header("玩家持有的金錢數量")]
    public int Money = 0;
    [Header("玩家持有的黃金數量")]
    public int Gold = 0;
    [Header("玩家持有的小麥數量")]
    public int Wheat = 0;
    [Header("玩家持有的玉米數量")]
    public int Corn = 0;
}