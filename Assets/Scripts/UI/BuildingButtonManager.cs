using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace galaxy890624
{
    /// <summary>
    /// 建築按鈕管理器 <br><br></br></br>
    /// 
    /// 功能： <br></br>
    /// - 自動掃描指定的按鈕容器（Content）下所有 Button（包含 inactive）； <br></br>
    /// - 使用按鈕名稱（預期格式：<c>ButtonPrefab_{BuildingData.name}</c>）來對應並綁定對應的 <see cref="BuildingData"/>; <br></br>
    /// - 綁定按鈕上的文字為建築顯示名稱，並將按鈕點擊事件連結至 <see cref="BuildPlacer"/> 開始放置建築。 <br></br>
    ///  <br></br>
    /// 注意事項： <br></br>
    /// - 需要在 Inspector 中指定 <see cref="buttonContainer"/> 與 <see cref="allBuildingDataList"/>; <br></br>
    /// - 若找不到對應的 <see cref="BuildingData"/>，會在 Console 顯示 Warning； <br></br>
    /// - 本類別不處理按鈕本身的可見/可點擊邏輯，只負責綁定資料與發送放置請求。 <br></br>
    /// </summary>
    public class BuildingButtonManager : MonoBehaviour
    {
        [Header("按鈕容器 (Content)")]
        /// <summary>
        /// 包含所有按鈕的容器（通常是 ScrollView 的 Content）。會對容器內所有子物件做 <see cref="GetComponentsInChildren{T}"/> 掃描（包含 inactive）。
        /// </summary>
        [SerializeField] private Transform buttonContainer;

        [Header("所有建築資料來源")]
        /// <summary>
        /// 編輯器指定的所有建築資料清單。用來建立名稱到 <see cref="BuildingData"/> 的查詢表。
        /// 每筆 <see cref="BuildingData"/> 的 <c>name</c> 欄位會被當作 key 使用（例如 "House_Lv1"）。
        /// </summary>
        [SerializeField] public List<BuildingData> allBuildingDataList = new List<BuildingData>();

        /// <summary>
        /// 以建築資源名稱為 key 的查詢表（由 <see cref="Awake"/> 建立）。
        /// key 應對應 <see cref="BuildingData.name"/>（例如 "House_Lv1"）。
        /// </summary>
        private Dictionary<string, BuildingData> buildingLookup;

        private void Awake()
        {
            if (buttonContainer == null)
            {
                Debug.LogError("<color=#ff00ff>[BuildingButtonManager.cs] 沒有指定 buttonContainer！</color>");
                return;
            }

            // 建立查找表
            buildingLookup = new Dictionary<string, BuildingData>();
            foreach (var data in allBuildingDataList)
            {
                if (data == null) continue;
                // 以 BuildingData 的名稱作為 key，例如 "House_Lv1"
                buildingLookup[data.name] = data;
            }

            // 掃描所有按鈕
            SetupAllButtons();
        }

        /// <summary>
        /// 掃描 Content 內的所有按鈕，根據按鈕名稱自動綁定對應的 <see cref="BuildingData"/>。
        /// </summary>
        /// <remarks>
        /// - 預期按鈕命名規則：<c>"ButtonPrefab_{BuildingData.name}"</c>。方法會把前綴 <c>"ButtonPrefab_"</c> 移除後再查表。
        /// - 若按鈕內有 <see cref="TMP_Text"/>，會將其文字改為 <see cref="BuildingData.GetDisplayName"/> 的回傳值。
        /// - 會先移除所有已有 listeners，確保只綁定此處建立的 callback。
        /// </remarks>
        private void SetupAllButtons()
        {
            Button[] buttons = buttonContainer.GetComponentsInChildren<Button>(true);
            Debug.Log($"<color=#ff00ff>[BuildingButtonManager.cs]</color> 找到 <color=#00ff00>{buttons.Length}</color> 顆按鈕");

            foreach (Button btn in buttons)
            {
                // 嘗試從名稱中提取建築關鍵字，例如 "ButtonPrefab_House_Lv1"
                string buttonName = btn.name.Replace("ButtonPrefab_", "").Trim();

                // 改進比對邏輯：支援精準 + 模糊兩種方式
                BuildingData matchedData = allBuildingDataList.Find(data =>
                    string.Equals(data.buildingID, buttonName, System.StringComparison.OrdinalIgnoreCase)
                    || btn.name.Contains(data.buildingID)
                );

                if (matchedData != null)
                {
                    // 更新建築名稱文字（找子物件 "BuildingName"）
                    TMP_Text text = btn.transform.Find("BuildingName")?.GetComponent<TMP_Text>();
                    if (text != null) text.text = matchedData.GetDisplayName();
                    // 更新建築圖示（找子物件 "BuildingImage"）
                    Image icon = btn.transform.Find("BuildingImage")?.GetComponent<Image>();
                    if (icon != null && matchedData.icon != null) icon.sprite = matchedData.icon;

                    // 綁定按下事件
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() => OnBuildingButtonClick(matchedData));

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
        /// 會檢查 <see cref="BuildPlacer.Instance"/> 是否存在，存在則呼叫 <see cref="BuildPlacer.StartPlacing(BuildingData)"/> 開始放置流程。
        /// </summary>
        /// <param name="data">要開始放置的建築資料。</param>
        private void OnBuildingButtonClick(BuildingData data)
        {
            if (BuildPlacer.Instance == null)
            {
                Debug.LogError("<color=#ff00ff>[BuildingButtonManager.cs] 找不到 <color=#ff00ff>BuildPlacer.Instance</color></color>");
                return;
            }

            Debug.Log($"<color=#ff00ff>[BuildingButtonManager.cs] 點擊建造: <color=#00ff00>{data.name}</color></color>");
            BuildPlacer.Instance.StartPlacing(data);
        }
    }
}