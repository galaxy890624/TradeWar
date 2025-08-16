using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 依 BuildingManager 的清單，動態產生建築按鈕 <br></br>
/// </summary>
public class BuildingCatalogUI : MonoBehaviour
{
    [Header("按鈕生成設定")]
    [SerializeField] private Transform contentRoot;   // 放按鈕的父物件（例如 ScrollView/Viewport/Content）
    [SerializeField] private GameObject buttonPrefab; // 內含 BuildingButtonEntry 的 Prefab
    [SerializeField] private bool clearOnBuild = true;

    private readonly List<GameObject> spawnedButtons = new();

    private void Start()
    {
        Rebuild();
    }

    [ContextMenu("Rebuild Buttons")]
    public void Rebuild()
    {
        if (contentRoot == null || buttonPrefab == null)
        {
            Debug.LogWarning("[BuildingCatalogUI] contentRoot 或 buttonPrefab 未指定");
            return;
        }

        if (clearOnBuild) ClearButtons();

        var list = BuildingManager.Instance.GetAllBuildings();
        foreach (var data in list)
        {
            var go = Instantiate(buttonPrefab, contentRoot);
            var entry = go.GetComponent<BuildingButtonEntry>();
            if (entry == null)
            {
                Debug.LogError("[BuildingCatalogUI] buttonPrefab 上缺少 BuildingButtonEntry 組件！");
                Destroy(go);
                continue;
            }

            entry.Init(data);
            spawnedButtons.Add(go);
        }
    }

    private void ClearButtons()
    {
        for (int i = spawnedButtons.Count - 1; i >= 0; i--)
        {
            if (spawnedButtons[i])
                Destroy(spawnedButtons[i]);
        }
        spawnedButtons.Clear();
    }
}