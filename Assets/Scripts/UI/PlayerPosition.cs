using UnityEngine;
using TMPro;

public class PlayerPosition : MonoBehaviour
{
    [Header("玩家座標的文字")]
    [SerializeField]
    private TextMeshProUGUI PlayerDataText;
    [Header("玩家的物件")]
    [SerializeField]
    private GameObject Player;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        PlayerDataText.text = " ( " + Player.transform.position.x.ToString("N3") + ", "
            + Player.transform.position.y.ToString("N3") + ", "
            + Player.transform.position.z.ToString("N3") + " ) ";
    }
}