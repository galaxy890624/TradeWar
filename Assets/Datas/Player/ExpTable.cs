using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "ExpTable", menuName = "Player Data/New ExpTable")]

public class ExpTable : ScriptableObject
{
    /// <summary>
    /// 升級所需經驗值 <br></br><br></br>
    /// 這裡使用的成長函數為 : y = 400 * ( 1.3 ^ x ) , 取整數 <br></br>
    /// x : 等級 <br></br>
    /// y : 升下一等級需要的經驗值 <br></br>
    /// </summary>
    public float[] MaxExp;
}
