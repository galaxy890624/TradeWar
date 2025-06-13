#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// �N�ثe�a�ϳ������e�g�J MapData �� MapInfo[] �t�m
/// </summary>
public class MapDataUpdater : MonoBehaviour
{
    [MenuItem("Tools/�a�Ϥu��/�N���������t�m�g�J MapData")]
    public static void ApplySceneToMapData()
    {
        MapManager manager = FindObjectOfType<MapManager>();
        if (manager == null)
        {
            Debug.LogError("�䤣�� MapManager�I");
            return;
        }

        // ���X MapData
        var mapDataField = manager.GetType().GetField("mapData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        MapData mapData = manager.MapData;

        if (mapData == null)
        {
            Debug.LogError("MapManager ���]�w MapData�I");
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

            // �M�� prefab ����
            for (int i = 0; i < mapData.MapPrefab.Length; i++)
            {
                if (mapData.MapPrefab[i] == null) continue;

                string prefabName = mapData.MapPrefab[i].name;
                if (objName == prefabName)
                {
                    mapData.SetTileObject(row, col, mapData.MapPrefab[i]);
                    break;
                }
            }
        }

        EditorUtility.SetDirty(mapData);
        AssetDatabase.SaveAssets();
        Debug.Log("<color=green>MapData �w��s���\�I�]�w�g�J MapInfo[]�^</color>");
    }
}
#endif