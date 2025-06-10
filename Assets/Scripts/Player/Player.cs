using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace galaxy890624
{
    /// <summary>
    /// 儲存玩家的基本功能 <br><br></br></br>
    /// 1. 玩家移動 <br></br>
    /// 2. 玩家動畫 <br></br>
    /// </summary>
    public class Player : MonoBehaviour
    {
        #region 玩家基本資料
        [field: Header("玩家移動設定")]
        [field: SerializeField, Range(0f, 10f)] public float MoveSpeed { get; private set; } = 5f; // 玩家移動速度
        [field: Header("玩家跳躍設定")]
        [field: SerializeField, Range(0f, 10f)] public float JumpForce { get; private set; } = 5f; // 玩家跳躍力量
        
        public Animator Animator { get; private set; } // 玩家動畫控制器
        public Rigidbody Rigidbody { get; private set; } // 玩家物理引擎
        [Header("地板判定")]
        [SerializeField] Vector3 GroundCheckOffset = new Vector3(0f, -0.1f, 0f); // 地板判定偏移量
        [SerializeField] private float GroundCheckRadius = 0.1f; // 地板判定半徑
        [SerializeField] private LayerMask GroundLayer; // 玩家碰撞地面圖層
        #endregion

        #region 玩家狀態
        public StateMachine StateMachine; // 狀態機實例
        public PlayerIdle PlayerIdle { get; private set; } // 玩家閒置狀態
        public PlayerWalk PlayerWalk { get; private set; } // 玩家移動狀態
        public PlayerJump PlayerJump { get; private set; } // 玩家跳躍狀態
        public PlayerFall PlayerFall { get; private set; } // 玩家攻擊狀態
        public PlayerFly PlayerFly { get; private set; } // 玩家飛行狀態
        #endregion

        private void OnDrawGizmos()
        {
            // 在編輯器中繪製地板判定區域
            Gizmos.color = new Color(1f, 0f, 1f, 0.5f);
            Gizmos.DrawSphere(transform.position + GroundCheckOffset, GroundCheckRadius);
        }

        /// <summary>
        /// 初始化 <br></br>
        /// 在一開始時 建立狀態機實例 與 狀態實例 <br></br>
        /// </summary>
        private void Awake()
        {
            // 不需要序列化了 (不用顯示在控制面板)
            Animator = GetComponent<Animator>();
            Rigidbody = GetComponent<Rigidbody>();

            // 實例化狀態機
            // 產生一個狀態機物件 在遊戲內開始執行
            // 與掛在物件上相同
            StateMachine = new StateMachine();
            // this 指的是 此類別 (這裡指的是 Player 類別)
            PlayerIdle = new PlayerIdle("玩家待機", this, StateMachine);
            PlayerWalk = new PlayerWalk("玩家移動", this, StateMachine);
            PlayerJump = new PlayerJump("玩家跳躍", this, StateMachine);
            PlayerFall = new PlayerFall("玩家落下", this, StateMachine);
            PlayerFly = new PlayerFly("玩家飛行", this, StateMachine);

            // 在一開始時 將 狀態機 指定 預設狀態 為 待機
            StateMachine.DefaultState(PlayerIdle);
        }

        /// <summary>
        /// 持續更新狀態 <br></br>
        /// </summary>
        private void Update()
        {
            StateMachine.UpdateState();
        }

        /// <summary>
        /// 設定玩家的移動速度 <br></br>
        /// </summary>
        /// <param name="Velocity">剛體的速度<br></br></param>
        public void SetVelocity(Vector3 Velocity)
        {
            Rigidbody.velocity = Velocity;
        }

        public bool IsGrounded()
        {
            // 檢查玩家是否接觸地面
            return Physics.CheckSphere(transform.position + GroundCheckOffset, GroundCheckRadius, GroundLayer);
        }
    }

}
