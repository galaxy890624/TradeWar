using UnityEngine;

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

            // 抓取玩家虛擬攝影機的 Transform (通常就是跟滑鼠連動的那個視角軸)
            // 這個相機控制的是 "你要走哪裡" 的方向基礎
            Transform VirtualCameraTransform = Camera.main.transform;

            // 人物模型也要跟著視角旋轉
            // VirtualCameraTransform.rotation.y = MouseRotation.MouseXInput;

            // 取得 Virtual Camera的 forward/right 向量
            Vector3 CameraForward = VirtualCameraTransform.forward;
            Vector3 CameraRight = VirtualCameraTransform.right;

            // 把相機的 forward/right 在 y 軸的高度去掉 (只保留水平方向)
            // 避免角色沿著 y 軸抬頭或下壓而偏離水平移動
            CameraForward.y = 0f;
            CameraRight.y = 0f;

            // 把方向向量正規化 ( 長度變 1 ) , 確保移動速度一致, 不會因角度不同而走快或走慢
            CameraForward.Normalize();
            CameraRight.Normalize();

            // 依照Virtual Camera的 forward/right 方向, 計算玩家的移動方向, 確保玩家移動方向永遠跟著攝影機
            Vector3 MoveDirection = CameraForward * VerticalInput + CameraRight * HorizontalInput;

            // 設定玩家的加速度
            // Player.Rigidbody.velocity.y 和 面板上的 gravity 會同時作用, 造成y座標迅速掉到-Infinity
            Player.SetVelocity( MoveDirection * Player.MoveSpeed );

            // 讓玩家面向相機的 forward 方向
            if (CameraForward.sqrMagnitude > 0.01f)
            {
                Quaternion TargetRotation = Quaternion.LookRotation(CameraForward);
                Player.PlayerVisualRoot.rotation = Quaternion.Slerp(Player.PlayerVisualRoot.rotation, TargetRotation, Time.deltaTime * 10f);
            }

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
