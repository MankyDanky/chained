using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [SerializeField] public float speed = 5f;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float sprintMulti = 3f;
    [SerializeField] private Transform playerBody;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController controller; 
    private float playerHeight = 1f;
    private float xRotation = 0f;
    private float yRotation = 0f;
    private bool canDash = true;
    private bool isDashing;
    private bool isSprinting;
    private float dashPower = 10f;
    private float dashTime = 0.2f;
    private float dashCD = 2.5f;
    private Transform gunPivot;
    private float gunPivotY;
    [SerializeField] float bobAmplitude;
    [SerializeField] private float bobSmoothing = 10f;
    private Vector3 targetGunPosition;

    // For CharacterController
    private Vector3 playerVelocity;
    private bool isGrounded;
    [SerializeField] private float gravity = -9.81f;
    public float jumpForce = 2f;

    public bool isWalking;
    public static Transform Instance;

    void Awake()
    {
        Instance = transform;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        animator = transform.Find("Model").GetComponent<Animator>();
        gunPivot = transform.Find("Main Camera/GunPivot");
        gunPivotY = gunPivot.localPosition.y;
        
        // Get the CharacterController component
        if (controller == null)
            controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (isDashing) { return; }

        // Check if grounded
        isGrounded = controller.isGrounded;
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f; // Small negative value to keep the controller grounded
        }

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

        // Sprint 
        float currentSpeed = speed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentSpeed = Mathf.Min(speed + sprintMulti, 12f);
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
        }

        // Handle movement
        if (forwardVelocity != 0 || rightVelocity != 0)
        {
            animator.SetBool("isWalking", true);
            isWalking = true;
            
            // Calculate movement direction
            Vector3 moveDirection = (transform.forward * forwardVelocity + transform.right * rightVelocity).normalized;
            
            // Move the character controller
            controller.Move(moveDirection * currentSpeed * Time.deltaTime);
            
            // Calculate gun bob
            float progress = animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;
            targetGunPosition = new Vector3(
                gunPivot.localPosition.x,
                gunPivotY + Mathf.Sin(progress * Mathf.PI * 4 * (isSprinting? 2f : 1f)) * bobAmplitude * (isSprinting? 2f : 1f),
                gunPivot.localPosition.z
            );
        }
        else
        {
            animator.SetBool("isWalking", false);
            isWalking = false;
            targetGunPosition = new Vector3(gunPivot.localPosition.x, gunPivotY, gunPivot.localPosition.z);
        }

        // Apply gravity
        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        // Smooth the gun position
        gunPivot.localPosition = Vector3.Lerp(gunPivot.localPosition, targetGunPosition, Time.deltaTime * bobSmoothing);

        // Jumping
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            playerVelocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        // Dash
        if (Input.GetKey(KeyCode.Q) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        
        Vector3 dashDirection = (transform.forward * Input.GetAxisRaw("Vertical") + transform.right * Input.GetAxisRaw("Horizontal")).normalized;
        
        if (dashDirection == Vector3.zero)
        {
            dashDirection = transform.forward;
        }
        
        float startTime = Time.time;
        while (Time.time < startTime + dashTime)
        {
            controller.Move(dashDirection * dashPower * Time.deltaTime);
            yield return null;
        }
        
        isDashing = false;
        yield return new WaitForSeconds(dashCD);
        canDash = true;
    }
}