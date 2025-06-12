using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace galaxy890624
{
    /// <summary>
    /// �ƹ����౱�� <br></br>
    /// �Ψӱ���a���������� <br></br>
    /// </summary>
    public class MouseRotation : MonoBehaviour
    {
        float MouseXInput = 0f;
        float MouseYInput = 0f;

        [Header("����������u (�q�`�O���a���Y��)")]
        [SerializeField] Transform HorizontalRotationAxis = null;
        [Header("����������u (�q�`�O���a������)")]
        [SerializeField] Transform VerticalRotationAxis = null;
        [Header("�ƹ��F�ӫ׳]�w")]
        [SerializeField, Range(1f, 5f)] public float MouseSensitivity = 3f; // �ƹ��F�ӫ׳]�w
        [Header("�����]�w")]
        [SerializeField, Range(-90f, 0f)] public float MinYAngle = -90f; // �̤p����
        [SerializeField, Range(0f, 90f)] public float MaxYAngle = 90f; // �̤j����


        private void Update()
        {
            // �u���b�C�����~�����
            MouseXInput = MouseXInput + (Input.GetAxis("Mouse X") * MouseSensitivity);
            MouseYInput = MouseYInput + (Input.GetAxis("Mouse Y") * -1f * MouseSensitivity);
            
            // ����Y���W�U����
            MouseYInput = Mathf.Clamp(MouseYInput, MinYAngle, MaxYAngle);

            // ��XYZ�ഫ������q

            // ��������q
            Quaternion HorizontalRotation = Quaternion.Euler(0f, MouseXInput, 0f);
            // �� ����������u �� �����
            HorizontalRotationAxis.localRotation = HorizontalRotation;

            // ��������q
            Quaternion VerticalRotation = Quaternion.Euler(MouseYInput, 0f, 0f);
            // �� ����������u �� �����
            VerticalRotationAxis.localRotation = VerticalRotation;
        }
    }

}