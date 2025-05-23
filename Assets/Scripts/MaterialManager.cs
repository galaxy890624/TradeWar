using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 快速將不同的 Base Map 紋理設置到多個材質上
/// </summary>
public class MaterialManager : MonoBehaviour
{
    public Material[] Materials; // 你的 n 個 Material
    public Texture2D[] BaseMaps; // n 個對應的 Base Map 紋理


    // Initialize
    void Awake()
    {
        if (Materials.Length != BaseMaps.Length)
        {
            print("<color=#ff00ff>材質數量和貼圖數量不匹配!</color>");
            return;
        }

        for (int i = 0; i < Materials.Length; i++)
        {
            if (Materials[i].HasProperty("_BaseMap")) // 確保有 Base Map 屬性
            {
                Materials[i].SetTexture("_BaseMap", BaseMaps[i]);
            }
            else
            {
                print($"<color=#ff00ff>Material <color=#00ff00>{Materials[i].name}</color> 沒有 _BaseMap 屬性</color>");
            }
        }
    }
}
