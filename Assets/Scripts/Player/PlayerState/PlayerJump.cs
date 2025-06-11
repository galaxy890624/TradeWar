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

        public override void Enter()
        {
            base.Enter();
            Player.SetVelocity(Player.Rigidbody.velocity + new Vector3(0f, Player.JumpForce, 0f)); // 設定玩家的跳躍速度
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void Update()
        {
            base.Update();

            // 在空中可以移動
            Player.SetVelocity(new Vector3(HorizontalInput * Player.MoveSpeed, Player.Rigidbody.velocity.y, VerticalInput * Player.MoveSpeed)); // 設定玩家的水平速度

            // 如果 剛體的速度小於等於0, 就切換到落下狀態
            if (Player.Rigidbody.velocity.y <= 0f)
            {
                // 切換到落下狀態
                StateMachine.SwitchState(Player.PlayerFall);
            }
        }
    }
}
