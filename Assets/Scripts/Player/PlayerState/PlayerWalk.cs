using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace galaxy890624
{
    /// <summary>
    /// 玩家行走狀態 <br></br>
    /// </summary>
    public class PlayerWalk : State
    {
        public PlayerWalk(string _StateName, Player _Player, StateMachine _StateMachine) : base(_StateName, _Player, _StateMachine)
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

            // 加速度
            Player.SetVelocity(new Vector3(HorizontalInput, Player.Rigidbody.velocity.y, VerticalInput) * Player.MoveSpeed);

            // 如果玩家沒有水平或垂直輸入, 則切換到待機狀態
            if (HorizontalInput == 0 && VerticalInput == 0)
            {
                // 切換狀態到 PlayerIdle
                StateMachine.SwitchState(Player.PlayerIdle);
            }
        }
    }

}
