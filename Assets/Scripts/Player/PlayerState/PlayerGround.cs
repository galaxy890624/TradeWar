using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace galaxy890624{
    /// <summary>
    /// 玩家在地面上的狀態 <br><br></br></br>
    /// 1. 玩家在地面上時的行為 <br></br>
    /// </summary>
    /// <remarks>
    /// 這個類別可以用來處理玩家在地面上的行為，例如移動、跳躍等。
    /// </remarks>
    /// </remarks>
    public class PlayerGround : State
    {
        public PlayerGround(string _StateName, Player _Player, StateMachine _StateMachine) : base(_StateName, _Player, _StateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void Update()
        {
            base.Update();
            // 如果玩家在地面上, 並且按空白鍵, 就切換到跳躍狀態
            if (Player.IsGrounded() && Input.GetKeyDown(KeyCode.Space))
            {
                // 切換到跳躍狀態
                StateMachine.SwitchState(Player.PlayerJump);
            }
        }
    }
}
