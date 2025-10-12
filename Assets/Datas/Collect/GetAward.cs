using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ObjectData", menuName = "Object Data/New Award Data")]
public class GetAward : ScriptableObject
{
    [Header("獲得經驗值")]
    public int GetExp = 0;
    [Header("獲得金錢")]
    public int GetMoney = 0;
    [Header("獲得黃金")]
    public int GetGold = 0;
    [Header("獲得木材")]
    public int GetWood = 0;
    [Header("獲得經驗值的音效")]
    public AudioSource GetExpSound;
    [Header("獲得金錢的音效")]
    public AudioSource GetMoneySound;
    [Header("獲得黃金的音效")]
    public AudioSource GetGoldSound;
    [Header("獲得木材的音效")]
    public AudioSource GetWoodSound;
}
