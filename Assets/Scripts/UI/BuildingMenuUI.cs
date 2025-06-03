using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingMenuUI : MonoBehaviour
{
    [Header("UI 設定")]
    public Transform buttonParent; // 垂直排列按鈕的 Panel
    public GameObject buttonPrefab; // 建築按鈕的預製件

    private void Start()
    {
        LoadMenu();
    }

    void LoadMenu()
    {
        List<BuildingData> allBuildings = BuildingManager.Instance.GetAllBuildings();

        foreach (var building in allBuildings)
        {
            GameObject btnObj = Instantiate(buttonPrefab, buttonParent);
            BuildingButton btn = btnObj.GetComponent<BuildingButton>();
            btn.Setup(building);
        }
    }
}