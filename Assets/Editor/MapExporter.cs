#if UNITY_EDITOR
using galaxy890624;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 將當前場景上的所有 MapTile 配置轉為 C# 程式碼（可貼回 MapData 初始化用）
/// </summary>
public class MapExporter : MonoBehaviour
{
    [MenuItem("Tools/地圖工具/匯出地圖配置程式碼")]
    public static void ExportMapConfiguration()
    {
        MapManager mapManager = FindObjectOfType<MapManager>();
        if (mapManager == null)
        {
            Debug.LogError("找不到 MapManager 物件！");
            return;
        }

        MapData mapData = mapManager.MapData;
        if (mapData == null)
        {
            Debug.LogError("MapManager 上未設定 MapData！");
            return;
        }

        var sb = new StringBuilder();

        foreach (MapTile tile in mapManager.GetComponentsInChildren<MapTile>())
        {
            int row = tile.row;
            int col = tile.col;

            if (tile.transform.childCount == 0) continue; // 沒有建築就跳過

            // 取第一個子物件（假設就是建築）
            Transform obj = tile.transform.GetChild(0);

            for (int i = 0; i < mapData.MapPrefab.Length; i++)
            {
                if (mapData.MapPrefab[i] == null) continue;

                string prefabName = mapData.MapPrefab[i].name;
                string instanceName = obj.name.Replace("(Clone)", "").Trim();

                if (instanceName == prefabName)
                {
                    sb.AppendLine($"mapData.SetTileObject({row}, {col}, mapData.MapPrefab[{i}]);");
                    break;
                }
            }
        }

        string result = sb.ToString();
        EditorGUIUtility.systemCopyBuffer = result;

        Debug.Log("<color=green>地圖配置已複製到剪貼簿！</color>\n" + result);
    }
}
#endif