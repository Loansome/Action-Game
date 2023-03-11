using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using KinematicCharacterController;
using Cinemachine;

public class InputManager : MonoBehaviour
{
    private PlayerAttack playerAttack;
    private CharacterMovement charaMove;
    private MyCharacterController characterController;
    private CharacterAnimation playerAnim;
    [SerializeField] private GameObject cinema;

    private PlayerInput playerInput;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction selectAction;
    private InputAction guardAction;
    private InputAction magicAction;
    private InputAction pauseAction;

    private void OnEnable()
	{
        //Debug.Log("Enabling");
        playerInput.enabled = true;
        //Debug.Log("Enabled");
	}

	private void OnDisable()
    {
        //Debug.Log("Disabling");
        playerInput.enabled = false;
        //Debug.Log("Disabled");
    }

	// Start is called before the first frame update
	void Awake()
    {
        //Debug.Log("Starting");
        playerInput = GetComponent<PlayerInput>();
        playerAttack = GetComponent<PlayerAttack>();
        charaMove = GetComponent<CharacterMovement>();
        characterController = GetComponent<MyCharacterController>();
        playerAnim = GetComponentInChildren<CharacterAnimation>();

        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        jumpAction = playerInput.actions["Jump"];
        selectAction = playerInput.actions["Select"];
        guardAction = playerInput.actions["Guard"];
        magicAction = playerInput.actions["Magic"];
        pauseAction = playerInput.actions["Pause"];

        Cursor.lockState = CursorLockMode.Locked;
        //Debug.Log("End starting");
    }

    // Update is called once per frame
    private void Update()
    {
        AttackUpdate();
        //Debug.Log("Updating");
        /*if (selectAction.triggered)
		{
            Debug.Log("Attack");
            playerAttack.SetAction(ComboState.JAB);
		}*/
        if (pauseAction.triggered)
        {
            cinema.GetComponent<CinemachineInputProvider>().enabled = !cinema.GetComponent<CinemachineInputProvider>().enabled;
        }

            AnimationUpdate();

        CharacterControllerUpdate();


        //CharaMoveUpdate();
    }

    private void AnimationUpdate()
	{
        if (moveAction.IsPressed()) playerAnim.Walk(true);
        else playerAnim.Walk(false);
	}

    private void CharacterControllerUpdate()
	{
        PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

        // Build the CharacterInputs struct
        characterInputs.MoveAxisForward = moveAction.ReadValue<Vector2>().y;
        characterInputs.MoveAxisRight = moveAction.ReadValue<Vector2>().x;
        characterInputs.CameraRotation = Camera.main.transform.rotation;
        characterInputs.JumpDown = jumpAction.triggered;
        //characterInputs.Attack = selectAction.triggered;
        //Debug.Log(characterInputs.JumpDown + " " + characterInputs.MoveAxisForward + " " + characterInputs.MoveAxisRight);

        // Apply inputs to character
        characterController.SetInputs(ref characterInputs);
	}

    private void AttackUpdate()
	{
        playerAttack.SetInputs(selectAction.triggered, guardAction.triggered, magicAction.triggered);
	}

    private void CharaMoveUpdate()
	{
        charaMove.SetJumpInput(jumpAction.triggered);
        charaMove.SetInputDirection(moveAction.ReadValue<Vector2>().x, moveAction.ReadValue<Vector2>().y);
	}
}
