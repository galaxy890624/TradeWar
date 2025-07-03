using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

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
            Debug.Log("<color=#0f7fff>State -> <color=#ff7f00>PlayerWalk</color></color>");
        }

        public override void Exit()
        {
            base.Exit();
        }
        
        public override void Update()
        {
            base.Update();

            // 設定玩家的加速度
            // Player.Rigidbody.velocity.y 和 面板上的 gravity 會同時作用, 造成y座標迅速掉到-Infinity
            Player.SetVelocity( MaxInput * Player.MoveSpeed );

            // 設定玩家的動畫
            Player.Animator.SetFloat("HorizontalDirection", HorizontalInput);
            Player.Animator.SetFloat("VerticalDirection", VerticalInput);

            // 如果玩家沒有水平或垂直輸入, 則切換到待機狀態
            if (HorizontalInput == 0 && VerticalInput == 0)
            {
                // 切換狀態到 PlayerIdle
                StateMachine.SwitchState(Player.PlayerIdle);
            }

        }
    }

}
