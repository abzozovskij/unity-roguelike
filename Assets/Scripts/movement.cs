using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class movement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    private float maxSpeed;
    public float groundDrag;

    public float sprintSpeed;
    public float sprintJumpForce;
    bool sprinting;

    public float jumpForce;
    private float maxJump;
    public float jumpCooldown;
    public float airMult;
    bool jumpReady;

    [Header("Crouch")]
    public float crouchSpeed = 4f;
    public float crouchHeight = 1.5f;

    public Transform camPos;
    private Vector3 camOriginalPos;

    private float originalHeight;
    private bool crouching;
    private bool canCrouch;
    private CapsuleCollider cCollider;
    private Vector3 originalCenter;

    [Header("Dash")]
    public float dashForce = 20f;
    public float dashCooldown = 0.2f;
    public float dashDuration = 0.2f;
    public bool dash = false;
    
    public int maxDashes = 3;
    public float dashRecharge = 2f;

    public bool dashDamageTrigger = false;
    public float dashDamage = 35f;
    public bool dashDamaged = false;
    private bool recharging = false;
    private int dashes;
    private bool dashReady = true;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask ground;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    [Header("UI Elements")]
    public TextMeshProUGUI dashesTxt;
    public Slider dashBar;
    Vector3 moveDirection;
    private BoxCollider bCollider;
    
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        jumpReady = true;
        dashes = maxDashes;
        maxSpeed = moveSpeed;
        maxJump = jumpForce;
        dashBar.maxValue = maxDashes;
        cCollider = GetComponentInChildren<CapsuleCollider>();
        bCollider = GetComponentInChildren<BoxCollider>();
        originalHeight = cCollider.height;
        originalCenter = cCollider.center;
        camOriginalPos = camPos.localPosition;
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, ground);

        SpeedControl();
        if (grounded)
        {
            rb.linearDamping = groundDrag;
            canCrouch = true;
        }
        else
        {
            rb.linearDamping = 0;
            canCrouch = false;
        }
        if (crouching)
        {
            moveSpeed = crouchSpeed;
        }
        else if (sprinting)
        {
            moveSpeed = sprintSpeed;
            jumpForce = sprintJumpForce;
        }
        else
        {
            moveSpeed = maxSpeed;
            jumpForce = maxJump;
        }
        dashesTxt.text = "Dashes: " + dashes;
        dashBar.value = dashes;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }
    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void jumpReset()
    {
        jumpReady = true;
    }
    public void OnMove(InputValue value) 
    { 
        Vector2 input = value.Get<Vector2>(); 
        horizontalInput = input.x; 
        verticalInput = input.y; 
    }
    public void OnJump(InputValue value) 
    { 
        if (value.isPressed && grounded && jumpReady) 
        { jumpReady = false; 
            Jump(); 
            Invoke(nameof(jumpReset), jumpCooldown); 
        } 
    }

    public void OnCrouch(InputValue value)
    {
        crouching = value.isPressed;

        if (canCrouch && crouching)
        {
            StartCrouch();
        }
        else
        {
            StopCrouch();
        }
    }

    void StartCrouch()
    {
        float heightDiff = originalHeight - crouchHeight;
        cCollider.height = crouchHeight;
        cCollider.center = originalCenter - new Vector3(0, heightDiff / 2f, 0);
        camPos.localPosition = camOriginalPos - new Vector3(0, heightDiff / 2f, 0);
    }

    void StopCrouch()
    {
        cCollider.height = originalHeight;
        cCollider.center = originalCenter;
        camPos.localPosition = camOriginalPos;
    }
    private void Dash()
    {
        dashReady = false;
        dash = true;
        dashes -= 1;
        if(dashDamageTrigger == true)
        {
            bCollider.enabled = true;
            dashDamaged = false;
        }

        Vector3 dashDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (dashDirection == Vector3.zero)
        {
            dashDirection = orientation.forward;
        }
            
        if (grounded)
        {
            rb.AddForce(dashDirection.normalized * dashForce, ForceMode.Impulse);
        }
        else
        {
            rb.AddForce(dashDirection.normalized * dashForce * airMult, ForceMode.Impulse);
        }

        Invoke(nameof(EndDash), dashDuration);
        Invoke(nameof(ResetDash), dashCooldown);
    }

    private void EndDash()
    {
        dash = false;
        bCollider.enabled = false;
        dashDamaged = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!dashDamageTrigger || dashDamaged)
        {
            return;
        }

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(dashDamage);
            dashDamaged = true;
            return;
        }

        EnemyBall ball = other.GetComponent<EnemyBall>();
        if (ball != null)
        {
            ball.TakeDamage(dashDamage);
            dashDamaged = true;
            return;
        }
    }

    private void ResetDash()
    {
        dashReady = true;

        if (dashes < maxDashes && !recharging)
        {
            recharging = true;
            Invoke(nameof(RechargeDash), dashRecharge);
        }
    }

    private void RechargeDash()
    {
        dashes += 1;
        if (dashes < maxDashes)
        {
            Invoke(nameof(RechargeDash), dashRecharge);
        }
        else
        {
            recharging = false;
        }
    }

    public void OnDash(InputValue value)
    {
        if (value.isPressed && dashReady && dashes > 0)
        {
            Dash();
        }
    }

    private void OnSprint(InputValue value)
    {
        sprinting = value.isPressed;
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMult, ForceMode.Force);
        }
    }

    public void AddDashes(int amount)
    {
        maxDashes += amount;
        dashes += amount;
        dashBar.maxValue = maxDashes;
    }

    private void SpeedControl()
    {
        Vector3 vel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        //calculate what the max velocity would be if going greater than the max movespeed and then force the player to not go faster
        if (vel.magnitude > moveSpeed && !dash)
        {
            Vector3 limVel = vel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limVel.x, rb.linearVelocity.y, limVel.z);
        }
    }

}
