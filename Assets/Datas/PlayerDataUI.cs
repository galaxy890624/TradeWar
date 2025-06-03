using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerDataUI : MonoBehaviour
{
    [Header("���a��ƪ���r")]
    [SerializeField]
    private TextMeshProUGUI PlayerDataText;
    [Header("���a��ƪ�����")]
    [SerializeField]
    private PlayerData PlayerData;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // PositionText.text = Player.transform.position.ToString("N3");
        PlayerDataText.text = " Money = " + "<color=#00ff00>" + PlayerData.Money.ToString("N0") + "</color>" + "\n"
            + " Gold = " + "<color=#00ff00>" + PlayerData.Gold.ToString("N0") + "</color>" + "\n"
            + " Wood = " + "<color=#00ff00>" + PlayerData.Wood.ToString("N0") + "</color>" + "\n";
    }
}