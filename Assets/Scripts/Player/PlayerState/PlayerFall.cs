using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace galaxy890624
{
    /// <summary>
    /// 玩家落下狀態 <br></br>
    /// </summary>
    public class PlayerFall : State
    {
        public PlayerFall(string _StateName, Player _Player, StateMachine _StateMachine) : base(_StateName, _Player, _StateMachine)
        {
        }
    }
}
