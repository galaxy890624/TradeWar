using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    public float Sensitivity = 2.0f;
    public float MinPitch = -90.0f;
    public float MaxPitch = 90.0f;
    public Transform Target; // 玩家角色

    private float Pitch = 0.0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float MouseX = Input.GetAxis("Mouse X");
        float MouseY = Input.GetAxis("Mouse Y");

        // 左右旋轉
        transform.Rotate(Vector3.up, MouseX * Sensitivity);

        // 上下旋轉
        Pitch -= MouseY * Sensitivity;
        Pitch = Mathf.Clamp(Pitch, MinPitch, MaxPitch);
        Target.localEulerAngles = new Vector3(Pitch, 0f, 0f); // 調整相機角度,繞x軸旋轉
    }

    void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}