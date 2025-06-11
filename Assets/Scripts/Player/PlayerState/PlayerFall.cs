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

            // 在空中可以移動
            Player.SetVelocity(new Vector3(HorizontalInput * Player.MoveSpeed, Player.Rigidbody.velocity.y, VerticalInput * Player.MoveSpeed)); // 設定玩家的水平速度

            // 如果 碰到地板, 就切換到待機狀態
            if (Player.IsGrounded())
            {
                // 切換到待機狀態
                StateMachine.SwitchState(Player.PlayerIdle);
            }
        }
    }
}
