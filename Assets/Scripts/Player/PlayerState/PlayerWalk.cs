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

        /*
        public override void Update()
        {
            base.Update();

            // 取得輸入
            Vector3 input = new Vector3(HorizontalInput, 0, VerticalInput);

            // 取得主攝影機的 Y 軸朝向
            Transform cam = Camera.main.transform;
            Vector3 camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 camRight = cam.right;

            // 依照攝影機方向計算移動向量
            Vector3 move = (camForward * input.z + camRight * input.x).normalized;

            // 設定速度
            Player.SetVelocity(new Vector3(move.x, Player.Rigidbody.velocity.y, move.z) * Player.MoveSpeed);

            // 玩家面向移動方向（可選）
            if (move.magnitude > 0.1f)
            {
                Player.transform.forward = move;
            }

            // 狀態切換
            if (HorizontalInput == 0 && VerticalInput == 0)
            {
                StateMachine.SwitchState(Player.PlayerIdle);
            }
        }*/
    }

}
