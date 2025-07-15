using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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

        [Header("自己的方向")]
        Quaternion Direction;

        /// <summary>
        /// 要求玩家面向某個方向 <br></br>
        /// 根據參照物來決定自身移動方向 <br></br>
        /// </summary>
        [Header("玩家面向的方向")]
        [SerializeField] Transform PlayerFace;

        [Header("確保玩家對齊方向")]
        /// <summary>要求我對齊我的母物件 <br></br></summary>
        public bool IsAlign = false;

        [Header("對齊速度")]
        [SerializeField] float AlignSpeed = 10f;

        [Header("玩家的Virtual Camera (通常用水平旋轉軸)")]
        [SerializeField] public Transform VirtualCameraTransform;

        [Header("滑鼠控制器")]
        [SerializeField] public MouseRotation MouseRotation;

        [Header("玩家視覺模型(Armature)")]
        [SerializeField] public Transform PlayerVisual;

        [Header("玩家視覺模型的節點")]
        [SerializeField] private Transform PlayerVisualRoot;

        /// <summary>
        /// 玩家動畫控制器 <br></br>
        /// </summary>
        public Animator Animator { get; private set; }
        /// <summary>
        /// 玩家物理引擎 <br></br>
        /// </summary>
        public Rigidbody Rigidbody { get; private set; }

        /// <summary>
        /// 玩家是否可以移動 <br></br>
        /// </summary>
        public bool CanMove { get; set; } = false;
        /// <summary>
        /// 玩家是否可以跳躍 <br></br>
        /// </summary>
        public bool CanJump { get; set; } = false;

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
            Animator = GetComponentInChildren<Animator>();
            Rigidbody = GetComponentInChildren<Rigidbody>();

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

            // 初始化的時候記住自己的方向
            Direction = this.transform.rotation;

            // 遊戲一開始時測試用 : 可以控制
            Debug.Log($"<color=#ff00ff>[Player.cs]CanMove = <color=#00ff00>{CanMove}</color></color>");
            Debug.Log($"<color=#ff00ff>[Player.cs]CanJump = <color=#00ff00>{CanJump}</color></color>");
            TestCanControl();

            // 初始化的時候 就自動尋找節點
            if (PlayerVisualRoot == null) PlayerVisualRoot = transform.Find("Armature");
        }

        /// <summary>
        /// 持續更新狀態 <br></br>
        /// </summary>
        private void Update()
        {
            StateMachine.UpdateState();

            //if (IsAlign) Direction = Quaternion.Lerp(Direction, PlayerFace.rotation, AlignSpeed);
            //this.transform.rotation = Direction;

            // 讓玩家移動的方向 和 滑鼠轉動的視角 一致
            this.transform.rotation = Quaternion.Euler(this.transform.rotation.x, MouseRotation.MouseXInput, this.transform.rotation.z); // = Player.transform.rotation

            // 目前Player的Rotation不會動, 眼睛的會動
            // 這裡顯示的是Quaternion, 不是Quaternion.Euler
            // Debug.Log($"<color=#ff00ff>[Player.cs] Player.transform.rotation = ( <color=#00ff00>{this.transform.rotation.x}, {this.transform.rotation.y}, {this.transform.rotation.z}</color> )</color>");

            // 測試 PlayerVisual 是否真的有被轉動, 這裡顯示的是 Quaternion.Euler
            Debug.Log($"<color=#ff00ff>[PlayerWalk] PlayerVisual.rotation.eulerAngles = <color=#00ff00>{PlayerVisual.rotation.eulerAngles}</color></color>");
        }

        /// <summary>
        /// 設定玩家的移動速度 <br></br>
        /// </summary>
        /// <param name="Velocity">剛體的速度<br></br></param>
        public void SetVelocity(Vector3 Velocity)
        {
            Rigidbody.velocity = Velocity;
            //Debug.Log($"<color=#ff00ff>Rigidbody.velocity = <color=#00ff00>{Velocity}</color></color>");
        }

        /// <summary>
        /// 設定玩家的移動方向 <br></br>
        /// 當視角左右旋轉的時候, 獲得transform的旋轉的Y值, 來改變人物移動的方向
        /// </summary>
        /// <param name="Direction">旋轉的Y值</param>
        public void SetDirection(Quaternion Direction)
        {
            transform.rotation = Direction;
        }
        /// <summary>
        /// 檢查 GameObject-玩家 是否碰到 Layer-Ground <br></br>
        /// 必須把地板群組的Layer 設定為 Ground
        /// </summary>
        /// <returns></returns>
        public bool IsGrounded()
        {
            // 檢查玩家是否接觸地面
            return Physics.CheckSphere(transform.position + GroundCheckOffset, GroundCheckRadius, GroundLayer);
        }

        /// <summary>
        /// 測試 : 可以控制 移動, 跳躍<br></br>
        /// </summary>
        private void TestCanControl()
        {
            CanMove = true;
            CanJump = true;
            Debug.Log($"<color=#ff00ff>[Player.cs]CanMove = <color=#00ff00>{CanMove}</color></color>");
            Debug.Log($"<color=#ff00ff>[Player.cs]CanJump = <color=#00ff00>{CanJump}</color></color>");
        }
    }
}
