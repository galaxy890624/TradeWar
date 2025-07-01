using UnityEngine;

namespace galaxy890624
{
    /// <summary>
    /// 狀態機的基礎類別 <br></br>
    /// 所有狀態都應繼承自此類別 <br></br>
    /// 包含 進入狀態 更新狀態 和 離開狀態 <br></br>
    /// </summary>

    public class State
    {
        private string StateName; // 用來記錄狀態名稱

        protected Player Player; // 用來記錄玩家實例
        protected StateMachine StateMachine; // 用來記錄狀態機實例

        protected float HorizontalInput; // 用來記錄水平輸入
        protected float VerticalInput; // 用來記錄垂直輸入
        protected Vector3 Movement; // 記錄玩家的輸入
        protected Vector3 MaxInput; // 讓速度輸入大小固定

        public State(string _StateName, Player _Player, StateMachine _StateMachine)
        {
            StateName = _StateName; // 設定狀態名稱
            Player = _Player; // 設定玩家實例
            StateMachine = _StateMachine; // 設定狀態機實例
        }

        /// <summary>
        /// 進入狀態時的邏輯 <br></br>
        /// </summary>
        public virtual void Enter() // virtual 代表這個方法可以被子類別覆寫
        {
            //Debug.Log($"<color=#ff00ff>進入 <color=#00ff00><{StateName}></color> 狀態</color>");
        }

        /// <summary>
        /// 更新狀態時的邏輯 <br></br>
        /// </summary>
        public virtual void Update() 
        {
            //Debug.Log($"<color=#ff00ff>更新 <color=#00ff00><{StateName}></color> 狀態</color>");
            HorizontalInput = Input.GetAxis("Horizontal"); // 獲取水平輸入
            VerticalInput = Input.GetAxis("Vertical"); // 獲取垂直輸入
            Movement = new Vector3 (HorizontalInput, 0f, VerticalInput);
            // 移動向量
            MaxInput = Vector3.ClampMagnitude(Movement, 1f); // 大小固定為1
            //Debug.Log($"<color=#ff00ff>HorizontalInput = <color=#00ff00>{HorizontalInput}</color>, VerticalInput = <color=#00ff00>{VerticalInput}</color></color>"); // 功能正常
        }

        /// <summary>
        /// 離開狀態時的邏輯 <br></br>
        /// </summary>
        public virtual void Exit() 
        {
            //Debug.Log($"<color=#ff00ff>離開 <color=#00ff00><{StateName}></color> 狀態</color>");
        }
    }

}
