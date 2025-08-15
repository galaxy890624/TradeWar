#if UNITY_EDITOR
using galaxy890624;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 將目前場景中的地圖內容更新回 MapData
/// </summary>
public class MapDataUpdater : MonoBehaviour
{
    [MenuItem("Tools/地圖工具/將場景中的配置寫入 MapData")]
    public static void ApplySceneToMapData()
    {
        MapManager manager = FindObjectOfType<MapManager>();
        if (manager == null)
        {
            Debug.LogError("找不到 MapManager！");
            return;
        }

        MapData mapData = manager.MapData;
        if (mapData == null)
        {
            Debug.LogError("MapManager 未設定 MapData！");
            return;
        }

        Undo.RecordObject(mapData, "Update MapData");

        foreach (MapTile tile in manager.GetComponentsInChildren<MapTile>())
        {
            int row = tile.row;
            int col = tile.col;

            if (tile.transform.childCount == 0)
            {
                mapData.ClearTile(row, col);
                continue;
            }

            GameObject child = tile.transform.GetChild(0).gameObject;
            string objName = child.name.Replace("(Clone)", "").Trim();

            for (int i = 0; i < mapData.MapPrefab.Length; i++)
            {
                if (mapData.MapPrefab[i] == null) continue;

                if (objName == mapData.MapPrefab[i].name)
                {
                    mapData.SetTileObject(row, col, mapData.MapPrefab[i]);
                    break;
                }
            }
        }

        EditorUtility.SetDirty(mapData);
        AssetDatabase.SaveAssets();
        Debug.Log("<color=green>MapData 已更新成功！（已寫入陣列資料）</color>");
    }
}
#endif