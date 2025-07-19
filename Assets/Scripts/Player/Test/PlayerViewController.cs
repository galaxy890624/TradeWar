using UnityEngine;

namespace galaxy890624
{
    /// <summary>
    /// 控制玩家視角與模型旋轉 ( 整合 : 滑鼠視角 + 模型面向 ) <br></br>
    /// </summary>
    public class PlayerViewController : MonoBehaviour
    {
        [Header("滑鼠靈敏度")]
        [Range(0.1f, 2f)] public float Sensitivity = 2f;

        [Header("仰角限制")]
        public float MinPitch = -90f;
        public float MaxPitch = 90f;

        [Header("控制對象")]
        public Transform PlayerBody;     // 控制水平旋轉的角色主體 (Y軸)
        public Transform CameraPivot;    // 控制垂直旋轉的相機或視角空物件 (X軸)

        private float Pitch = 0f;

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            float MouseX = Input.GetAxis("Mouse X") * Sensitivity;
            float MouseY = Input.GetAxis("Mouse Y") * Sensitivity;

            // 控制角色本體水平旋轉（左/右）
            if (PlayerBody != null) PlayerBody.Rotate(Vector3.up, MouseX);

            // 控制相機上下視角（仰角）
            if (CameraPivot != null)
            {
                Pitch -= MouseY;
                Pitch = Mathf.Clamp(Pitch, MinPitch, MaxPitch);
                CameraPivot.localRotation = Quaternion.Euler(Pitch, 0f, 0f);
            }
        }

        private void OnDisable()
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}