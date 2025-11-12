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
            //Debug.Log("<color=#0f7fff><color=#ff00ff>[PlayerJump.cs] </color>State -> <color=#ff7f00>PlayerJump</color></color>");
            Player.SetVelocity(Player.Rigidbody.linearVelocity + new Vector3(0f, Player.JumpForce, 0f)); // 設定玩家的跳躍速度
            Player.Animator.SetBool("IsGrounded", false);
            Player.Animator.SetFloat("Gravity", 1f);
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void Update()
        {
            base.Update();

            // 在空中可以移動
            Player.SetVelocity(new Vector3(HorizontalInput * Player.MoveSpeed, Player.Rigidbody.linearVelocity.y, VerticalInput * Player.MoveSpeed)); // 設定玩家的水平速度
            Player.Animator.SetFloat("HorizontalDirection", HorizontalInput);
            Player.Animator.SetFloat("VerticalDirection", VerticalInput);

            // PlayerJump -> PlayerFall
            // 如果 剛體的速度小於等於0, 就切換到落下狀態
            if (Player.Rigidbody.linearVelocity.y <= 0f) StateMachine.SwitchState(Player.PlayerFall);
        }
    }
}
