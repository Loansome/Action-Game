using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public CharacterController controller;
    public float _defaultMoveSpeed;
    public float _moveSpeed = 10f;
    public float _slowedSpeed = 2f;
    public float _sprintSpeed = 18f;
    public float _gravity = -9.81f;
    public float _jumpHeight = 3f;

    public float _attackForward = 0f;
    public bool isAttacking = false;

    [Header("Ground Check Settings")]
    public Transform groundCheck;
    public float _groundDistance = -0.4f; //radius of sphere
    public LayerMask groundMask; //checks for collision with the floor specifically, in case it catches player collision first, which it will

    float turnSmoothVelocity, turnSmoothTime = 0.025f;

    Vector3 inputDirection, moveDirection, jumpVelocity;
    float targetAngle, smoothedAngle, fallJumpSpeed;
    public bool isGrounded = false;
    int jumpTimes = 0;
    private CharacterAnimation playerAnim;

    private Animator _rootAnim;

    private void Start()
    {
        _rootAnim = GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterController>();
        playerAnim = GetComponentInChildren<CharacterAnimation>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        //isGrounded = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, .8f);
    }

    void Update()
    {
        //Debug.Log(isGrounded ? "GROUNDED" : "NOT GROUNDED");
        isGrounded = Physics.CheckSphere(groundCheck.position, _groundDistance, groundMask); //checks for collision with floor using a small invisible sphere; returns true/false
        playerAnim.IsGrounded(isGrounded);

        Move();

        SimulateGravity();

        ResetVelocity();

        /*if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || jumpTimes < 2)) // checks if player can jump
        {
            fallJumpSpeed = 1f;
            jumpVelocity.y = jumpHeight;
            jumpTimes++;
        }
        if (jumpVelocity.y < 0)
        {
            fallJumpSpeed = .75f;
            if (isGrounded)
            {
                jumpTimes = 0;
            }
        }
        if (jumpVelocity.y < -gravityValue / 1.75f) jumpVelocity.y = -gravityValue / 1.75f;
        jumpVelocity.y -= gravityValue * fallJumpSpeed * Time.deltaTime;

        controller.Move(jumpVelocity * Time.deltaTime);*/
    }

    void Move()
    {
        /*
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _moveSpeed = _defaultMoveSpeed;
        }*/

        inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized; // get direction player is moving, returns 1 if moving that direction
        if (inputDirection.magnitude >= .1f && !isAttacking)
        {
            targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y; // where object should rotate to (keeps movement to camera focus)
            smoothedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime); // smooth between rotation
            transform.rotation = Quaternion.Euler(0f, smoothedAngle, 0f); // rotate object (smoothly) towards movement direction
            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward; // movement relative to angle (set to current smoothed angle instead of final angle)

            controller.Move(moveDirection.normalized * _moveSpeed * Time.deltaTime); // move player where they are inputting
            playerAnim.Walk(true);
        } else playerAnim.Walk(false);
        //if (isAttacking)
        //{
            _attackForward = _rootAnim.GetFloat("RootForward");

            controller.Move(transform.forward * _attackForward * _moveSpeed * Time.deltaTime);
        //}
        //if (_attackForward > 0) _attackForward += _gravity / 8f * Time.deltaTime; // forward movement from attack slowly dwindles or resets once hitting the ground
        //if (_attackForward < 0 || isGrounded) _attackForward = 0;
    }

    public Vector3 GetInputDirection()
	{
        return inputDirection;
	}

    void SimulateGravity()
    {
        jumpVelocity.y += _gravity * Time.deltaTime; //need a velocity variable to simulate real gravity
        controller.Move(jumpVelocity * Time.deltaTime); //multiply times deltaTime twice, as is shown on velocity equation
        if (jumpVelocity.z > 0) jumpVelocity.z += _gravity / 8f * Time.deltaTime;
        else if (jumpVelocity.z < 0 || isGrounded) jumpVelocity.z = 0;
        if (Input.GetButtonDown("Jump") && (isGrounded || jumpTimes < 2) && !isAttacking)
        {
            jumpVelocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity); //force required to jump according to physics. (Square root of jump height x (-2) x gravity)
            //_moveSpeed = _sprintSpeed;
            jumpTimes++;
            //if (jumpTimes == 2) _attackForward = 20; // double flight forward boost
            playerAnim.Jump(true);
        }
        else if (Input.GetButtonUp("Jump") || isGrounded) playerAnim.Jump(false);
    }

    public void AirAttackJump(float attackHeight, bool gravityOff)
    {
        //if (Input.GetButtonDown("Fire1") && (!isGrounded)) {
            if (!gravityOff) jumpVelocity.y = Mathf.Sqrt(attackHeight * -2f * _gravity); //force required to jump according to physics. (Square root of jump height x (-2) x gravity)
            else jumpVelocity.y = attackHeight;
        //playerAnim.Attack();
        //}
    }

    void ResetVelocity()
    {
        /*if (!isGrounded && jumpVelocity.y < 0)
            _moveSpeed -= 0.01f;*/

        if (isGrounded && jumpVelocity.y < 0) //when touching the ground, AND when velocity is at all greater than 0 (meaning player is being pushed by gravity), reset the velocity
        {
            jumpVelocity.y = -2f; //sticks player to ground   
            //_moveSpeed = _defaultMoveSpeed;
            jumpTimes = 0;
        }
    }

    /*private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, Vector3.down);
    }*/
}

