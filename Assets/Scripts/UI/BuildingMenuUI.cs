using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingMenuUI : MonoBehaviour
{
    [Header("UI �]�w")]
    public Transform buttonParent; // �����ƦC���s�� Panel
    public GameObject buttonPrefab; // �ؿv���s���w�s��

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