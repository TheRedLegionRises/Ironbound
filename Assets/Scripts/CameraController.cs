using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;

    [Header("Camera Boundaries")]
    public float minX = -10f;
    public float maxX = 10f;
    public float minZ = -10f;
    public float maxZ = 10f;

    private InputActions inputActions;
    
    void Awake()
    {
        inputActions = new InputActions();
    }

    void OnEnable()
    {
        inputActions.Camera.Enable();
    }

    void OnDisable()
    {
        inputActions.Camera.Disable();
    }

    void Update()
    {
        Vector2 movement =
            inputActions.Camera.Move.ReadValue<Vector2>();

        // Convert input into world movement
        Vector3 moveDirection =
            transform.right * movement.x +
            Vector3.ProjectOnPlane(
                transform.forward,
                Vector3.up
            ).normalized * movement.y;

        // Prevent faster diagonal movement
        moveDirection = moveDirection.normalized;

        transform.position +=
            moveDirection * moveSpeed * Time.deltaTime;

        // Apply camera boundaries
        Vector3 position = transform.position;

        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.z = Mathf.Clamp(position.z, minZ, maxZ);

        transform.position = position;
    }
}
