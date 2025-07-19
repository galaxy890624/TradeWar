using UnityEngine;

/// <summary>
/// 滑鼠控制角色本體與相機旋轉（水平+垂直）
/// </summary>
public class PlayerLookController : MonoBehaviour
{
    [Header("要旋轉的角色本體（水平Y軸）")]
    [SerializeField] private Transform CharacterBody;
    [Header("要旋轉的相機（垂直X軸）")]
    [SerializeField] private Transform CameraPivot;
    [Header("滑鼠靈敏度")]
    [SerializeField] private float Sensitivity = 2.0f;
    [Header("仰角限制")]
    [SerializeField] private float MinPitch = -90f;
    [SerializeField] private float MaxPitch = 90f;

    private float Pitch = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * Sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * Sensitivity;

        // 水平旋轉角色
        if (CharacterBody != null) CharacterBody.Rotate(Vector3.up, mouseX);

        // 垂直旋轉相機
        if (CameraPivot != null)
        {
            Pitch -= mouseY;
            Pitch = Mathf.Clamp(Pitch, MinPitch, MaxPitch);
            CameraPivot.localEulerAngles = new Vector3(Pitch, 0f, 0f);
        }
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}