using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace galaxy890624
{
    /// <summary>
    /// 木頭採集 <br><br></br></br>
    /// - 掛在木頭的Prefab上 <br></br>
    /// - 如果玩家碰到木頭，則採集木頭 <br></br>
    /// - 採集木頭後，木頭消失 <br></br>
    /// - 玩家獲得木頭數量 +1 <br></br>
    /// </summary>
    public class WoodCollector : MonoBehaviour
    {
        [SerializeField, Header("玩家物件")] private GameObject Player;
        [SerializeField, Header("玩家資料")] private PlayerData PlayerData;
        [SerializeField, Header("GetAward的ScriptableObj")] private GetAward GetAward;

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.name == "Player")
            {
                // 玩家獲得木頭的對應數量
                PlayerData.Wood += GetAward.GetWood;
                // 木頭消失
                Destroy(this.gameObject);
            }
        }
    }
}

