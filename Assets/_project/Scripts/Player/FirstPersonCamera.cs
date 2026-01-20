using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonCamera : MonoBehaviour
{
    [Header("References")]
    public Transform cameraPivot;

    [Header("Settings")]
    public float sensitivity = 0.1f;
    public float minY = -80f;
    public float maxY = 80f;

    float pitch; // X
    float yaw;   // Y

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yaw = transform.eulerAngles.y;
    }

    void Update()
    {
        Vector2 lookDelta = Mouse.current.delta.ReadValue();

        float mouseX = lookDelta.x * sensitivity;
        float mouseY = lookDelta.y * sensitivity;

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minY, maxY);

        yaw += mouseX;

        // Aplica rotações ABSOLUTAS
        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
    }
}
