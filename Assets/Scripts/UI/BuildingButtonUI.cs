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
        [SerializeField] private TMP_Text descText; // 升級建築描述文字

        private BuildingData buildingData;

        /// <summary>
        /// 初始化按鈕顯示資料 <br></br>
        /// </summary>
        public void Initialize(BuildingData data)
        {
            buildingData = data;
            // 建築分類 與 等級 的文字
            nameText.text = $"<color=#ff00ff>{data.category} <color=#00ff00>Lv{data.level}</color></color>";
            descText.text = $"<color=#ff00ff>花費：<color=#00ff00>{data.costMoney}</color>，大小：<color=#00ff00>{data.width}×{data.height}</color></color>";

            if (data.icon != null) iconImage.sprite = data.icon;
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
