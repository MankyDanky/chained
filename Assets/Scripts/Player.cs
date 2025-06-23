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
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject Brute;
    [SerializeField] private GameObject SpawnPoint;
    private float playerHeight = 1f;
    private float xRotation = 0f;
    private float yRotation = 0f;
    private bool canDash = true;
    private bool isDashing;
    private float dashPower = 96f;
    private float dashTime = 0.2f;
    private float dashCD = 2.5f;
    public float jumpForce = 5f;
    private Transform gunPivot;
    private float gunPivotY;
    [SerializeField] float bobAmplitude;
    [SerializeField] private float bobSmoothing = 1f; // Add this line
    private Vector3 targetGunPosition; // Add this line

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
        //Inputs for wasd
        float xaxis = Input.GetAxisRaw("Horizontal");
        float yaxis = Input.GetAxisRaw("Vertical");
        // Adjust camera and player rotation
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        float forwardVelocity = yaxis;
        float rightVelocity = xaxis;

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
        //Check if the player is moving
        if (forwardVelocity != 0 || rightVelocity != 0)
        {
            //Animates
            animator.SetBool("isWalking", true);
            isWalking = true;
            Vector3 moveDirection = (transform.forward * forwardVelocity + transform.right * rightVelocity).normalized;
            rb.linearVelocity = new Vector3(moveDirection.x * speed, rb.linearVelocity.y, moveDirection.z * speed);
            float progress = animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;
            
            // Calculate target position with bob
            targetGunPosition = new Vector3(
                gunPivot.localPosition.x,
                gunPivotY + Mathf.Sin(progress * Mathf.PI * 4) * bobAmplitude,
                gunPivot.localPosition.z
            );
        }
        else
        {
            animator.SetBool("isWalking", false);
            isWalking = false;
            targetGunPosition = new Vector3(gunPivot.localPosition.x, gunPivotY, gunPivot.localPosition.z);
        }

        // Smooth the gun position
        gunPivot.localPosition = Vector3.Lerp(gunPivot.localPosition, targetGunPosition, Time.deltaTime * bobSmoothing);

        // Jumping
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position + new Vector3(0, playerHeight + 100, 0), Vector3.down, out hit, 100f))
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
        Debug.DrawRay(playerCamera.transform.position + new Vector3(0, playerHeight + 100, 0), Vector3.down, Color.red);

        //Dash
        if (Input.GetKey(KeyCode.Q) && canDash == true)
        {
            StartCoroutine(Dash());
        }
        if(Input.GetKey(KeyCode.C))
        {
            Instantiate(Brute, SpawnPoint.transform.position, Quaternion.identity);
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