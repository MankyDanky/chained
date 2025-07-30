using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FirstPersonController : MonoBehaviour
{
    Camera mainCamera;
    Camera weaponCamera;
    float targetFOV = 60f;
    public float health = 100f;
    public float maxHealth = 100f;
    [SerializeField] public float speed = 5f;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float sprintMulti = 3f;
    [SerializeField] private Transform playerBody;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController controller;
    [SerializeField] private HealthBar healthBar;
    private float playerHeight = 1f;
    private float xRotation = 0f;
    private float yRotation = 0f;
    private bool canDash = true;
    private bool isDashing;
    private bool isSprinting;
    private float dashPower = 20f;
    private float dashTime = 0.2f;
    private float dashCooldown = 2.5f;
    private Transform gunPivot;
    private float gunPivotY;
    private float gunPivotX;
    private float gunPivotZ;
    [SerializeField] float bobAmplitude;
    [SerializeField] private float bobSmoothing = 10f;
    private Vector3 targetGunPosition;
    Shake shake;
    public bool isDead = false;
    public bool inCutscene = false;

    // For CharacterController
    [SerializeField] private Vector3 playerVelocity;
    public bool isGrounded;
    [SerializeField] private float gravity = -9.81f;
    public float jumpForce = 2f;

    public bool isWalking;
    public static Transform Instance;
    Animator gameOverScreen;

    // Sounds
    [SerializeField] GameObject dashSound;
    [SerializeField] GameObject landSound;

    void Awake()
    {
        Instance = transform;
    }

    void Start()
    {
        mainCamera = Camera.main;
        weaponCamera = mainCamera.transform.Find("WeaponCamera").GetComponent<Camera>();
        shake = mainCamera.GetComponent<Shake>();
        healthBar = GameObject.Find("/Canvas/HealthBar").GetComponent<HealthBar>();
        Cursor.lockState = CursorLockMode.Locked;
        animator = transform.Find("Model").GetComponent<Animator>();
        gunPivot = transform.Find("Main Camera/GunPivot");
        gunPivotY = gunPivot.localPosition.y;
        gunPivotX = gunPivot.localPosition.x;
        gunPivotZ = gunPivot.localPosition.z;
        controller = GetComponent<CharacterController>();
        gameOverScreen = GameObject.Find("/GameOverScreen").GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead || inCutscene) return;
        // Check if grounded
        isGrounded = controller.isGrounded;
        if (isGrounded && playerVelocity.y < 0)
        {
            if (playerVelocity.y < -2.2f)
            {
                Instantiate(landSound, transform.position, Quaternion.identity);
            }
            playerVelocity.y = -2f;
        }

        // Get mouse input (only if not dashing)
        if (!isDashing)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            // Adjust camera and player rotation
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        }

        float forwardVelocity = 0;
        float rightVelocity = 0;

        // Only allow movement input if not dashing
        if (!isDashing)
        {
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
        }

        // Sprint (only if not dashing)
        float currentSpeed = speed;
        if (!isDashing && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {
            targetFOV = 70f;
            currentSpeed = Mathf.Min(speed + sprintMulti, 12f);
            isSprinting = true;
            animator.SetFloat("WalkSpeed", 2);
        }
        else
        {
            isSprinting = false;
            animator.SetFloat("WalkSpeed", 1);
            if (!isDashing) { targetFOV = 60f; }
        }

        // Handle movement (only if not dashing)
        if (!isDashing && (forwardVelocity != 0 || rightVelocity != 0))
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
                gunPivotY + Mathf.Sin(progress * Mathf.PI * 4) * bobAmplitude * (isSprinting ? 2f : 1f),
                gunPivot.localPosition.z
            );
        }
        else if (!isDashing)
        {
            animator.SetBool("isWalking", false);
            isWalking = false;
            targetGunPosition = new Vector3(gunPivot.localPosition.x, gunPivotY, gunPivot.localPosition.z);
        }

        // Apply gravity
        if (!isGrounded)
        {
            playerVelocity.y += gravity * Time.deltaTime;
        }
        controller.Move(playerVelocity * Time.deltaTime);

        // Apply gun movement on jump/fall
        if (!isGrounded)
        {
            targetGunPosition = new Vector3(
                gunPivot.localPosition.x,
                gunPivotY - Mathf.Clamp(playerVelocity.y, -5, 5) * 0.01f, // Adjust the y position based on vertical velocity
                gunPivot.localPosition.z
            );
        }

        Vector3 planeVelocity = new Vector3(playerVelocity.x, 0, playerVelocity.z);
        if (planeVelocity.magnitude > 1f)
        {
            float localVelocityX = Vector3.Dot(planeVelocity, transform.right);
            float localVelocityZ = Vector3.Dot(planeVelocity, transform.forward);
            targetGunPosition = new Vector3(
                gunPivotX - localVelocityX * 0.01f,
                targetGunPosition.y,
                gunPivotZ - localVelocityZ * 0.01f
            );
        }

        // Smooth the gun position
        gunPivot.localPosition = Vector3.Lerp(gunPivot.localPosition, targetGunPosition, Time.deltaTime * bobSmoothing);

        // Apply camera FOV
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFOV, Time.deltaTime * 5f);
        weaponCamera.fieldOfView = Mathf.Lerp(weaponCamera.fieldOfView, targetFOV, Time.deltaTime * 5f);

        // Jumping (only if not dashing)
        if (!isDashing && isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            playerVelocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        // Damping
        if (playerVelocity.x > 0)
        {
            playerVelocity.x -= playerVelocity.x * Time.deltaTime * 5f;
            if (playerVelocity.x < 0.01f) { playerVelocity.x = 0; }
        }
        else if (playerVelocity.x < 0)
        {
            playerVelocity.x -= playerVelocity.x * Time.deltaTime * 5f;
            if (playerVelocity.x > -0.01f) { playerVelocity.x = 0; }
        }
        if (playerVelocity.z > 0)
        {
            playerVelocity.z -= playerVelocity.z * Time.deltaTime * 5f;
            if (playerVelocity.z < 0.01f && playerVelocity.z > -0.01f) { playerVelocity.z = 0; }
        }
        else if (playerVelocity.z < 0)
        {
            playerVelocity.z -= playerVelocity.z * Time.deltaTime * 5f;
            if (playerVelocity.z > -0.01f && playerVelocity.z < 0.01f) { playerVelocity.z = 0; }
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
        targetFOV = 80f;
        Instantiate(dashSound, transform.position, Quaternion.identity);
        Vector3 dashDirection = (transform.forward * Input.GetAxisRaw("Vertical") + transform.right * Input.GetAxisRaw("Horizontal")).normalized;

        if (dashDirection == Vector3.zero)
        {
            dashDirection = transform.forward;
        }

        // Apply dash impulse to playerVelocity
        Vector3 dashVelocity = dashDirection * dashPower;
        playerVelocity.x = dashVelocity.x;
        playerVelocity.z = dashVelocity.z;

        yield return new WaitForSeconds(dashTime);
        targetFOV = 60f;
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;
        health -= amount;
        healthBar.UpdateHealthBar();
        shake.start = true;
        shake.startHurt = true;
        if (health <= 0)
        {
            StartCoroutine(Die());
        }
    }

    public void ApplyImpulse(Vector3 force)
    {
        playerVelocity += force;
    }

    IEnumerator Die()
    {
        isDead = true;
        Volume globalVolume = FindAnyObjectByType<Volume>();
        DepthOfField depthOfField;
        ColorAdjustments colorAdjustments;
        globalVolume.profile.TryGet<DepthOfField>(out depthOfField);
        globalVolume.profile.TryGet<ColorAdjustments>(out colorAdjustments);
        depthOfField.active = true;
        colorAdjustments.colorFilter.overrideState = true;
        colorAdjustments.colorFilter.Override(Color.white);
        depthOfField.focalLength.Override(0.1f);
        float elapsedTime = 0f;
        while (elapsedTime < 0.8f)
        {
            elapsedTime += Time.deltaTime;
            Time.timeScale = Mathf.Lerp(1f, 0f, elapsedTime);
            depthOfField.focalLength.Override(Mathf.Lerp(0.1f, 300f, elapsedTime));
            colorAdjustments.colorFilter.Override(Color.Lerp(Color.white, Color.gray, elapsedTime));
            yield return null;
        }
        Time.timeScale = 0f;
        Debug.Log("Player died");
        yield return new WaitForSecondsRealtime(1f);
        gameOverScreen.SetTrigger("GameOver");
        Cursor.lockState = CursorLockMode.None;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Fence"))
        {
            ElectricFence electricFence = hit.gameObject.GetComponent<ElectricFence>();
            ApplyImpulse(Vector3.up * 10f + (transform.position - hit.point).normalized * 20f);
            TakeDamage(10f);
            Instantiate(electricFence.electricEffect, electricFence.transform.position + electricFence.transform.forward * 2.2f + Vector3.up * 1.5f, Quaternion.identity);
        }
    }

    public void IncreaseHealth(float amount)
    {
        maxHealth += amount;
        health += amount;
        healthBar.UpdateHealthBar();
    }
}