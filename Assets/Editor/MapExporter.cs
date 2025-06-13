#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Text;

/// <summary>
/// �N��e�����W���Ҧ� MapTile �t�m�ର C# �{���X
/// </summary>
public class MapExporter : MonoBehaviour
{
    [MenuItem("Tools/�a�Ϥu��/�ץX�a�ϰt�m�{���X")]
    public static void ExportMapConfiguration()
    {
        MapManager mapManager = FindObjectOfType<MapManager>();
        if (mapManager == null)
        {
            Debug.LogError("�䤣�� MapManager ����I");
            return;
        }

        MapData mapData = mapManager.MapData;

        if (mapData == null)
        {
            Debug.LogError("MapManager �W���]�w MapData�I");
            return;
        }

        var sb = new StringBuilder();

        foreach (MapTile tile in mapManager.GetComponentsInChildren<MapTile>())
        {
            int row = tile.row;
            int col = tile.col;

            if (tile.transform.childCount == 0) continue; // �S������N���L

            Transform obj = tile.transform.GetChild(0); // �Ĥ@�Ӥl����

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

        // �ƻs��ŶKï
        EditorGUIUtility.systemCopyBuffer = result;
        Debug.Log("<color=green>�a�ϰt�m�w�ƻs��ŶKï�I</color>\n" + result);
    }
}
#endif