using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour
{
    [Header("UI 元件")]
    public Image iconImage;
    public Text nameText;
    public Button button;

    private BuildingData data;

    public void Setup(BuildingData buildingData)
    {
        data = buildingData;
        iconImage.sprite = data.icon;
        nameText.text = data.GetDisplayName();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        BuildPlacer.Instance.StartPlacing(data);
    }
}
