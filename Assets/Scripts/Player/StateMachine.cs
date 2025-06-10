using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace galaxy890624
{
    /// <summary>
    /// 狀態機 <br><br></br></br>
    /// 1. 預設狀態 <br></br>
    /// 2. 狀態轉換 <br></br>
    /// </summary>
    public class StateMachine
    {
        /// <summary>
        /// 當前狀態 <br></br>
        /// </summary>
        private State CurrentState;

        /// <summary>
        /// 預設狀態 <br></br>
        /// </summary>
        /// <param name="DefaultState">要指定的預設狀態 <br></br></param>
        public void DefaultState(State DefaultState)
        {
            // Debug.Log($"<color=#ff00ff>設定預設狀態為 <color=#00ff00><{state.GetType().Name}></color></color>");
            // 在這裡可以設定預設狀態的邏輯

            // 當前狀態 -> 預設狀態
            CurrentState = DefaultState;
            // 進入 當前狀態
            CurrentState.Enter();
        }
        /// <summary>
        /// 更新狀態 <br></br>
        /// </summary>
        public void UpdateState()
        {
            // Debug.Log($"<color=#ff00ff>更新狀態 <color=#00ff00><{state.GetType().Name}></color></color>");
            // 在這裡可以設定狀態更新的邏輯

            // 更新 當前狀態
            CurrentState.Update();
        }

        /// <summary>
        /// 切換狀態 <br></br>
        /// </summary>
        /// <param name="NewState">要切換的新狀態 <br></br></param>
        public void SwitchState(State NewState)
        {
            // Debug.Log($"<color=#ff00ff>切換狀態到 <color=#00ff00><{state.GetType().Name}></color></color>");
            // 在這裡可以設定狀態轉換的邏輯

            // 離開 當前狀態
            CurrentState.Exit();
            // 當前狀態 -> 新狀態
            CurrentState = NewState;
            // 進入 新狀態
            CurrentState.Enter();
        }
    }

}
