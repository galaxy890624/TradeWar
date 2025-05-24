using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ObjectData", menuName = "Object Data/New Award Data")]
public class GetAward : ScriptableObject
{
    [Header("獲得經驗值")]
    public float GetExp = 0;
    [Header("獲得金錢")]
    public float GetMoney = 0;
    [Header("獲得黃金")]
    public float GetGold = 0;
    [Header("獲得小麥")]
    public float GetWheat = 0;
    [Header("獲得玉米")]
    public float GetCorn = 0;
    [Header("獲得經驗值的音效")]
    public AudioSource GetExpSound;
    [Header("獲得金錢的音效")]
    public AudioSource GetMoneySound;
    [Header("獲得黃金的音效")]
    public AudioSource GetGoldSound;
    [Header("獲得小麥的音效")]
    public AudioSource GetWheatSound;
    [Header("獲得玉米的音效")]
    public AudioSource GetCornSound;
}
