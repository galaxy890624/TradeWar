using UnityEngine;

namespace galaxy890624
{
    /// <summary>
    /// 玩家待機狀態 <br></br>
    /// </summary>
    public class PlayerIdle : PlayerGround
    {
        public PlayerIdle(string _StateName, Player _Player, StateMachine _StateMachine) : base(_StateName, _Player, _StateMachine)
        {
        }
        /// <summary>
        /// Override 覆寫 <br><br></br></br>
        /// 覆寫父類別有 virtual 關鍵字的成員 <br></br>
        /// </summary>
        public override void Enter()
        {
            base.Enter();
            //Debug.Log("<color=#0f7fff><color=#ff00ff>[PlayerIdle.cs]</color> State -> <color=#ff7f00>PlayerIdle</color></color>");
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void Update()
        {
            base.Update();

            // 如果 不能移動 就跳出 Update()
            //if (!Player.CanMove) return;

            // PlayerIdle -> PlayerWalk
            // 如果玩家有水平或垂直輸入, 則切換到移動狀態
            if (HorizontalInput != 0 || VerticalInput != 0)
            {
                // 切換狀態到 PlayerWalk
                StateMachine.SwitchState(Player.PlayerWalk);
            }
        }
    }

}

