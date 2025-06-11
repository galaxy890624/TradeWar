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
    MapManager MapManager;

    [Header("Map Tile Rows")]
    [SerializeField, Range(0, 1000)] public int Rows = 0;
    [Header("Map Tile Cols")]
    [SerializeField, Range(0, 1000)] public int Cols = 0;
    [Header("地圖塊物件的間隔")]
    [SerializeField, Range(0f, 20f)] public float MapSpacing = 10.0f;
    [Header("子物件的2D列表")]
    // 這裡不吃GameObject[][], 也不吃List<GameObject<GameObject>>
    [SerializeField] public GameObject[] MapInfo;
}