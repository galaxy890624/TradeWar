using UnityEngine;

namespace galaxy890624
{
    public class PlayerVisualRotator : MonoBehaviour
    {
        [Header("角色模型（要被旋轉的）")]
        [SerializeField] private Transform VisualTarget;

        [Header("視角來源（滑鼠控制的旋轉軸）")]
        [SerializeField] private Transform CameraDirectionSource;

        [Header("是否反向（模型預設面對-Z）")]
        [SerializeField] private bool Flip180 = false;

        [Header("轉向速度")]
        [SerializeField] private float RotateSpeed = 10f;

        private void Update()
        {
            if (VisualTarget == null || CameraDirectionSource == null) return;

            Vector3 Forward = CameraDirectionSource.forward;
            Forward.y = 0f;

            if (Forward.sqrMagnitude < 0.01f) return;

            Quaternion TargetRotation = Quaternion.LookRotation(Forward);

            if (Flip180) TargetRotation *= Quaternion.Euler(0, 180f, 0); // 若模型預設面對-Z，加180補償

            VisualTarget.rotation = Quaternion.Slerp( VisualTarget.rotation, TargetRotation, Time.deltaTime * RotateSpeed );

            Debug.DrawRay(transform.position, transform.forward * 2f, Color.red);
        }
    }
}