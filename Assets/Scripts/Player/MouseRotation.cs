using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace galaxy890624
{
    /// <summary>
    /// 滑鼠旋轉控制 <br></br>
    /// 用來控制玩家的視角旋轉 <br></br>
    /// </summary>
    public class MouseRotation : MonoBehaviour
    {
        float MouseXInput = 0f;
        float MouseYInput = 0f;

        [Header("水平旋轉視線 (通常是玩家的頭部)")]
        [SerializeField] Transform HorizontalRotationAxis = null;
        [Header("垂直旋轉視線 (通常是玩家的眼睛)")]
        [SerializeField] Transform VerticalRotationAxis = null;
        [Header("滑鼠靈敏度設定")]
        [SerializeField, Range(1f, 5f)] public float MouseSensitivity = 3f; // 滑鼠靈敏度設定
        [Header("仰角設定")]
        [SerializeField, Range(-90f, 0f)] public float MinYAngle = -90f; // 最小仰角
        [SerializeField, Range(0f, 90f)] public float MaxYAngle = 90f; // 最大仰角


        private void Update()
        {
            // 只有在遊戲中才能旋轉
            MouseXInput = MouseXInput + (Input.GetAxis("Mouse X") * MouseSensitivity);
            MouseYInput = MouseYInput + (Input.GetAxis("Mouse Y") * -1f * MouseSensitivity);
            
            // 限制Y的上下角度
            MouseYInput = Mathf.Clamp(MouseYInput, -MaxYAngle, -MinYAngle);

            // 把XYZ轉換成旋轉量

            // 水平旋轉量
            Quaternion HorizontalRotation = Quaternion.Euler(0f, MouseXInput, 0f);
            // 更換 水平旋轉視線 的 旋轉值
            HorizontalRotationAxis.localRotation = HorizontalRotation;

            // 垂直旋轉量
            Quaternion VerticalRotation = Quaternion.Euler(MouseYInput, 0f, 0f);
            // 更換 垂直旋轉視線 的 旋轉值
            VerticalRotationAxis.localRotation = VerticalRotation;
        }
    }

}