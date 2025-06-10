using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace galaxy890624
{
    /// <summary>
    /// 玩家跳躍狀態 <br></br>
    /// </summary>
    public class PlayerJump : State
    {
        public PlayerJump(string _StateName, Player _Player, StateMachine _StateMachine) : base(_StateName, _Player, _StateMachine)
        {
        }
    }
}
