using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 附加在實體建築上，用來記錄這個建築屬於哪一筆 BuildingData <br></br>
/// 可用來做動畫、升級邏輯、產出等擴充行為
/// </summary>
public class Building : MonoBehaviour
{
    public BuildingData data;

    public void Init(BuildingData buildingData)
    {
        data = buildingData;
        name = data.GetDisplayName();
    }
}
