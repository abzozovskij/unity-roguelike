using UnityEngine;
using UnityEngine.InputSystem;

public class movement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMult;
    bool jumpReady;


    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask ground;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        jumpReady = true;

    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, ground);
        SpeedControl();
        if (grounded)
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = 0;
        }
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

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMult, ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
        Vector3 vel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if(vel.magnitude > moveSpeed)
        {
            //calculate what the max velocity would be if going greater than the max movespeed and then force the player to not go faster
            Vector3 limVel = vel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limVel.x, rb.linearVelocity.y, limVel.z);
        }
    }

}
