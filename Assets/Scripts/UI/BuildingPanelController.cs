using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace galaxy890624
{
    /// <summary>
    /// 控制建築面板中所有建築按鈕的生成與綁定
    /// </summary>
    public class BuildingPanelController : MonoBehaviour
    {
        [Header("按鈕預製物 & 插入位置")]
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private Transform contentRoot;

        [Header("建築資料列表")]
        [SerializeField] private List<BuildingData> buildingList;

        private void Start()
        {
            foreach (var data in buildingList)
            {
                GameObject go = Instantiate(buttonPrefab, contentRoot);
                var ui = go.GetComponent<BuildingButtonUI>();
                ui.Initialize(data);

                Button btn = go.GetComponent<Button>();
                btn.onClick.AddListener(ui.OnClick); // 綁定點擊事件
            }
        }
    }
}

