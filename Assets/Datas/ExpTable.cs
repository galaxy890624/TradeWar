using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "ExpTable", menuName = "Player Data/New ExpTable")]

public class ExpTable : ScriptableObject
{
    /// <summary>
    /// 升級所需經驗值
    /// </summary>
    public float[] MaxExp;
}
