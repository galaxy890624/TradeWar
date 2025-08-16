using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 一顆建築按鈕的顯示與點擊邏輯 <br></br>
/// </summary>
public class BuildingButtonEntry : MonoBehaviour
{
    [Header("UI 參考")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text subText;   // 顯示花費 / 尺寸
    [SerializeField] private Button button;

    private BuildingData data;

    /// <summary> 由清單生成器呼叫，灌入資料 <br></br> </summary>
    public void Init(BuildingData buildingData)
    {
        data = buildingData;

        if (iconImage) iconImage.sprite = data.icon;
        if (titleText) titleText.text = $"{data.category}  Lv {data.level}";
        if (subText) subText.text = $"$ {data.costMoney}　木材 {data.costWood}　黃金 {data.costGold}　尺寸 {data.width}×{data.height}";

        if (button)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }
    }

    private void OnClick()
    {
        if (data == null) return;
        BuildingManager.Instance.SelectBuilding(data.category, data.level);
    }
}