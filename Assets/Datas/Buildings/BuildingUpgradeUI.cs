using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 升級與拆除選單 UI
/// </summary>
public class BuildingUpgradeUI : MonoBehaviour
{
    public static BuildingUpgradeUI Instance { get; private set; }

    public GameObject panel;
    public Button upgradeButton;
    public Button demolishButton;

    private Building currentBuilding;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);

        // 綁定事件
        upgradeButton.onClick.AddListener(OnUpgrade);
        demolishButton.onClick.AddListener(OnDemolish);
    }

    public void Show(Building building)
    {
        currentBuilding = building;
        panel.SetActive(true);
        panel.transform.position = Input.mousePosition; // 顯示在滑鼠位置
    }

    public void Hide()
    {
        panel.SetActive(false);
        currentBuilding = null;
    }

    void OnUpgrade()
    {
        currentBuilding.Upgrade();
        Hide();
    }

    void OnDemolish()
    {
        currentBuilding.Demolish();
    }
}