using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    public float sensitivity = 2.0f;
    public float minPitch = -45.0f;
    public float maxPitch = 45.0f;
    public Transform target; // 玩家角色

    private float pitch = 0.0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // 左右旋转
        transform.Rotate(Vector3.up, mouseX * sensitivity);

        // 上下旋转
        pitch -= mouseY * sensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        target.localEulerAngles = new Vector3(pitch, 0, 0); // 调整相机角度，绕X轴旋转
    }

    void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}