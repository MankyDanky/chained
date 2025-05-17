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
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;
    private float playerHeight = 1f;
    private float xRotation = 0f;
    private float yRotation = 0f;
    private bool canDash = true;
    private bool isDashing;
    private float dashPower = 96f;
    private float dashTime = 0.2f;
    private float dashCD = 2.5f;

    public bool isWalking;


    public static Transform Instance;


    void Awake()
    {
        Instance = transform;
    }



    public float jumpForce = 5f;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        animator = transform.Find("Model").GetComponent<Animator>();
    }

    void FixedUpdate()
    {
    }

    void Update()
    {
        //if dashing lock movement
        if (isDashing) { return; }

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

        //Sprint 
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            speed = speed + sprintMulti;
            if (speed > 12f)
            {
                speed = 12f;
            }
        }
        else
        {
            speed = 5;
        }

        if (forwardVelocity != 0 || rightVelocity != 0)
        {
            animator.SetBool("isWalking", true);
            isWalking = true;
            Vector3 moveDirection = (transform.forward * forwardVelocity + transform.right * rightVelocity).normalized;
            rb.linearVelocity = new Vector3(moveDirection.x * speed, rb.linearVelocity.y, moveDirection.z * speed);
        }
        else
        {
            animator.SetBool("isWalking", false);
            isWalking = false;
        }




        // Jumping
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position - new Vector3(0, playerHeight / 2, 0), Vector3.down, out hit, 0.5f))
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }


        //Dash
        if (Input.GetKey(KeyCode.Q) && canDash == true)
        {
            StartCoroutine(Dash());
        }
    }
    //Dash mechanic
    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        rb.useGravity = false;

        Vector3 dashDirection = (transform.forward * Input.GetAxisRaw("Vertical") + transform.right * Input.GetAxisRaw("Horizontal")).normalized;

        if (dashDirection == Vector3.zero)
        {
            dashDirection = transform.forward;
        }

        rb.linearVelocity = dashDirection * dashPower;

        yield return new WaitForSeconds(dashTime);
        rb.useGravity = true;
        isDashing = false;
        yield return new WaitForSeconds(dashCD);
        canDash = true;
    }
}