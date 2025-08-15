using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace galaxy890624
{
    /// <summary>
    /// 建築 UI 控制器 - 動態生成按鈕，並與 BuildPlacer 綁定 <br></br>
    /// </summary>
    public class BuildingUIManager : MonoBehaviour
    {
        [Header("UI 設定")]
        [SerializeField] private Transform buttonParent; // 按鈕容器
        [SerializeField] private GameObject buttonPrefab; // 按鈕預製件

        private void Start()
        {
            LoadButtons();
        }

        /// <summary>
        /// 生成所有建築按鈕
        /// </summary>
        private void LoadButtons()
        {
            List<BuildingData> allBuildings = BuildingManager.Instance.GetAllBuildings();

            foreach (var data in allBuildings)
            {
                GameObject btnObj = Instantiate(buttonPrefab, buttonParent);

                // UI 元件
                Image iconImage = btnObj.transform.Find("Icon").GetComponent<Image>();
                TMP_Text nameText = btnObj.transform.Find("Name").GetComponent<TMP_Text>();
                TMP_Text descText = btnObj.transform.Find("Desc").GetComponent<TMP_Text>();

                if (data.icon != null) iconImage.sprite = data.icon;
                nameText.text = $"<color=#ff00ff>{data.category} <color=#00ff00>Lv{data.level}</color></color>";
                descText.text = $"花費: {data.costMoney} | 大小: {data.width}×{data.height}";

                Button btn = btnObj.GetComponent<Button>();
                btn.onClick.AddListener(() =>
                {
                    BuildPlacer.Instance.StartPlacing(data);
                });
            }
        }
    }
}