using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace galaxy890624
{
    /// <summary>
    /// 建築選單中的單一建築按鈕控制 <br></br>
    /// </summary>
    public class BuildingButtonUI : MonoBehaviour
    {
        [Header("BuildingData 的 ScriptableObj")]
        [SerializeField] private BuildingData BuildingData;
        [Header("UI 元件")]
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descText;

        private BuildingData buildingData;

        /// <summary>
        /// 初始化按鈕顯示資料 <br></br>
        /// </summary>
        public void Initialize(BuildingData data)
        {
            buildingData = data;

            nameText.text = $"{data.category} Lv{data.level}";
            descText.text = $"花費：{data.costMoney}，大小：{data.width}×{data.height}";

            if (data.icon != null)
                iconImage.sprite = data.icon;
        }

        /// <summary>
        /// 當按鈕被點擊，呼叫建造模式 <br></br>
        /// </summary>
        public void OnClick()
        {
            if (BuildPlacer.Instance != null)
            {
                BuildPlacer.Instance.StartPlacing(buildingData);
            }
            else
            {
                Debug.LogWarning("BuildPlacer.Instance 不存在！");
            }
        }
    }

}
