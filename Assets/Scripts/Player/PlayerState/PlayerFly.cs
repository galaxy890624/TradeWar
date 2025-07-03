using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace galaxy890624
{
    /// <summary>
    /// 玩家飛行狀態 <br></br>
    /// </summary>
    public class PlayerFly : State
    {
        public PlayerFly(string _StateName, Player _Player, StateMachine _StateMachine) : base(_StateName, _Player, _StateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            Debug.Log("<color=#0f7fff>State -> <color=#ff7f00>PlayerFly</color></color>");
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void Update()
        {
            base.Update();
        }
    }

}