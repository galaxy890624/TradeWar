using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace galaxy890624
{
    /// <summary>
    /// 玩家行走狀態 <br></br>
    /// </summary>
    public class PlayerWalk : PlayerGround
    {
        public PlayerWalk(string _StateName, Player _Player, StateMachine _StateMachine) : base(_StateName, _Player, _StateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            Debug.Log("<color=#0f7fff><color=#ff00ff>[PlayerWalk.cs]</color> State -> <color=#ff7f00>PlayerWalk</color></color>");
        }

        public override void Exit()
        {
            base.Exit();
            
        }
        
        public override void Update()
        {
            base.Update();

            // 取得 Virtual Camera Transform
            Transform VirtualCameraTransform = Player.VirtualCameraTransform;

            // 取得Virtual Camera的 forward/right 方向, 忽略y軸
            Vector3 CameraForward = VirtualCameraTransform.forward;
            Vector3 CameraRight = VirtualCameraTransform.right;
            CameraForward.y = 0f;
            CameraRight.y = 0f;
            CameraForward.Normalize(); // 將 forward 方向歸一化
            CameraRight.Normalize(); // 將 right 方向歸一化

            // 依照Virtual Camera的 forward/right 方向, 計算玩家的移動方向, 確保玩家移動方向永遠跟著攝影機
            Vector3 MoveDirection = CameraForward * VerticalInput + CameraRight * HorizontalInput;

            // 設定玩家的加速度
            // Player.Rigidbody.velocity.y 和 面板上的 gravity 會同時作用, 造成y座標迅速掉到-Infinity
            Player.SetVelocity( MoveDirection * Player.MoveSpeed );

            // 設定玩家的動畫
            Player.Animator.SetFloat("HorizontalDirection", HorizontalInput);
            Player.Animator.SetFloat("VerticalDirection", VerticalInput);

            // PlayerWalk -> PlayerIdle
            // 如果玩家沒有水平或垂直輸入, 則切換到待機狀態
            if (HorizontalInput == 0 && VerticalInput == 0)
            {
                // 切換狀態到 PlayerIdle
                StateMachine.SwitchState(Player.PlayerIdle);
            }
        }
    }

}
