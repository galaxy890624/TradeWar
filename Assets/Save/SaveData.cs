using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 整體存檔結構：包含所有建築
/// </summary>
[Serializable]
public class SaveData
{
    public List<BuildingSaveData> buildings = new();
}

/// <summary>
/// 單一建築存檔資訊
/// </summary>
[Serializable]
public class BuildingSaveData
{
    public BuildingCategory category;
    public int level;
    public float rotationY;
    public int row;
    public int col;
}