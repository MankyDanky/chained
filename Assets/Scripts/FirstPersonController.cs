using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [SerializeField] public float speed = 5f;
    [SerializeField] private float mouseSensitivity = 2f;   
    [SerializeField] private Transform playerBody;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Rigidbody rb;
    private float playerHeight = 1f;
    private float xRotation = 0f;
    private float yRotation = 0f;
    public float jumpForce = 5f;
    public static Transform Instance;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Awake()
    {
        Instance = transform;
    }

    void Update()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Adjust camera and player rotation
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        float forwardVelocity = 0;
        float rightVelocity = 0;

        if (Input.GetKey(KeyCode.W))
        {
            forwardVelocity = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            forwardVelocity = -1;
        }

        if (Input.GetKey(KeyCode.A))
        {
            rightVelocity = -1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rightVelocity = 1;
        }

        // Calculate the movement direction
        Vector3 moveDirection = (transform.forward * forwardVelocity + transform.right * rightVelocity).normalized;
        rb.linearVelocity = new Vector3(moveDirection.x * speed, rb.linearVelocity.y, moveDirection.z * speed);
    
        // Jumping
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position - new Vector3(0, playerHeight/2, 0), Vector3.down, out hit, 0.5f))
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
    }
}
