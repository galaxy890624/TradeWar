using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace galaxy890624
{
    /// <summary>
    /// 建築按鈕管理器
    /// - 掃描 Content 下所有 Button（包含 inactive）
    /// - 依名稱自動綁定 BuildingData
    /// - 綁定顯示名稱、圖示、點擊事件
    /// </summary>
    public class BuildingButtonManager : MonoBehaviour
    {
        [Header("按鈕容器 (Content)")]
        [Tooltip("包含所有按鈕的容器（通常是 ScrollView 的 Content）")]
        [SerializeField] private Transform buttonContainer;

        [Header("所有建築資料來源")]
        [Tooltip("包含所有可建造建築的 BuildingData")]
        [SerializeField] public List<BuildingData> allBuildingDataList = new List<BuildingData>();

        [Header("建造控制器 (BuildPlacer)")]
        [Tooltip("若不指定，會自動使用 BuildPlacer.Instance")]
        [SerializeField] private BuildPlacer buildPlacer;

        private void Awake()
        {
            if (buttonContainer == null)
            {
                Debug.LogError("<color=#ff00ff>[BuildingButtonManager.cs]</color> 沒有指定 buttonContainer！");
                return;
            }

            if (buildPlacer == null) buildPlacer = BuildPlacer.Instance;

            SetupAllButtons();
        }

        /// <summary>
        /// 掃描 Content 內所有按鈕並綁定
        /// </summary>
        private void SetupAllButtons()
        {
            if (buttonContainer == null)
            {
                Debug.LogError("<color=#ff00ff>[BuildingButtonManager.cs] Button Container 未設定！</color>");
                return;
            }

            Button[] buttons = buttonContainer.GetComponentsInChildren<Button>(true);
            Debug.Log($"<color=#ff00ff>[BuildingButtonManager.cs]</color> 找到 <color=#00ff00>{buttons.Length}</color> 顆按鈕");

            foreach (Button btn in buttons)
            {
                string buttonName = btn.name.Replace("ButtonPrefab_", "").Trim();

                // 支援精準與模糊比對
                BuildingData matchedData = allBuildingDataList.Find(data =>
                    data != null &&
                    (string.Equals(data.name, buttonName, System.StringComparison.OrdinalIgnoreCase)
                    || string.Equals(data.buildingID, buttonName, System.StringComparison.OrdinalIgnoreCase)
                    || btn.name.Contains(data.name)
                    || (!string.IsNullOrEmpty(data.buildingID) && btn.name.Contains(data.buildingID)))
                );

                if (matchedData != null)
                {
                    // 設定顯示名稱
                    TMP_Text text = btn.transform.Find("BuildingName")?.GetComponent<TMP_Text>();
                    if (text != null) text.text = matchedData.GetDisplayName();

                    // 設定圖示
                    Image icon = btn.transform.Find("BuildingImage")?.GetComponent<Image>();
                    if (icon != null && matchedData.icon != null) icon.sprite = matchedData.icon;

                    // 綁定按鈕事件
                    btn.onClick.RemoveAllListeners();
                    var capturedData = matchedData;
                    btn.onClick.AddListener(() => OnBuildingButtonClick(capturedData));

                    Debug.Log($"<color=#ff00ff>[BuildingButtonManager.cs]</color> 綁定按鈕 <color=#00ff00>{btn.name}</color> → <color=#00ff00>{matchedData.displayName}</color>");
                }
                else
                {
                    Debug.LogWarning($"<color=#ff00ff>[BuildingButtonManager.cs]</color> 找不到對應的 BuildingData，按鈕名稱：<color=#00ff00>{btn.name}</color>");
                }
            }
        }

        /// <summary>
        /// 按下建築按鈕時的處理流程。
        /// 呼叫 BuildPlacer 進入建造模式。
        /// </summary>
        private void OnBuildingButtonClick(BuildingData data)
        {
            if (buildPlacer == null)
            {
                Debug.LogError("<color=#ff00ff>[BuildingButtonManager.cs]</color> 找不到 BuildPlacer 實例！");
                return;
            }
            // 注意 : 有沒有其他透明 UI（如 Panel、Image）覆蓋在按鈕上方？這會攔截點擊。
            // 可暫時隱藏所有 Panel/Image 測試。
            Debug.Log($"<color=#ff00ff>[BuildingButtonManager.cs]</color> 點擊建造: <color=#00ff00>{data.name}</color>");

            if (buildPlacer.buildingUIRoot != null && !buildPlacer.buildingUIRoot.activeSelf) buildPlacer.buildingUIRoot.SetActive(true);

            buildPlacer.StartPlacing(data);
        }
    }
}