using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;

public enum ComboState
{
    NONE,
    ATTACK,
    DODGE,
    MAGIC
}

public class PlayerAttack : MonoBehaviour
{

    private CharacterAnimation playerAnimation;

    private bool inAttack;
    private float attackDuration;
    private float comboTime = 1;
    private float cancelTime;
    public ComboState currentComboState;

    public CharacterMovement charaMove;
    public MyCharacterController characterController;
    public ComboChainHandler comboChain;
    public AttackContextHandler contextHandler;

    public Transform attackPoint;
    public Vector3 attackRange;
    public LayerMask enemyLayers;

    [Header("Actions")]
    public AbilityHolder equippedAbilities;
    public AttackData currentAttack;
    public AnimationData currentAnimation;

    private bool inputAttack;
    private bool inputDodge;
    private bool inputMagic;

    void Awake()
    {
        playerAnimation = GetComponentInChildren<CharacterAnimation>();
        charaMove = GetComponent<CharacterMovement>();
        characterController = GetComponent<MyCharacterController>();
        comboChain = GetComponent<ComboChainHandler>();
        contextHandler = GetComponent<AttackContextHandler>();
    }

    private void Start()
    {
        currentComboState = ComboState.NONE;
        currentAttack = null;
    }

    // Update is called once per frame
    void Update()
    {
        CheckAttack();

        if (currentAttack)
            ManageEvents();

        ResetComboState();
    }

    private void ManageEvents()
	{
        if ( currentAttack.gravityTimeframe.x <= comboTime && comboTime <= currentAttack.gravityTimeframe.y)
        {
            characterController.GravityMod = currentAttack.gravityMod;
        }
        else characterController.GravityMod = 1;


    }


    public void SetInputs(bool attack, bool dodge, bool magic)
	{
        inputAttack = attack;
        inputDodge = dodge;
        inputMagic = magic;
	}

	#region StateMachine

	private void CheckAttack()
	{
        if (inputAttack) // check if player attacks
        {
            Attack();
        }
        else if (inputDodge) // check if player dodges
        {
            Dodge();
        }
    }

    private void None() { }

    private void Attack() {
        if ((currentComboState == ComboState.ATTACK && cancelTime <= comboTime || currentComboState == ComboState.NONE)) // check if player attacks
        {
            AttackData newAttack = comboChain.TryNextAttack(!characterController.IsGrounded());
            if (newAttack != null)
            {
                comboChain.SetCurrentAttack(newAttack);
                SetAttack(newAttack);
                Debug.Log("Attack");
                currentComboState = ComboState.ATTACK;
            }
        }
    }

    private void Dodge() {
        if (equippedAbilities.IsEquipped("Dodge Roll") && (currentComboState == ComboState.NONE && characterController.IsGrounded())) // check if player dodges
        {
            SetAnimation("Dodge Roll");
            Debug.Log("Dodge");
            currentComboState = ComboState.DODGE;
        }
    }
    private void Magic() {
        if (inputMagic)
        {
            if (currentComboState != ComboState.NONE)
                return;

            currentComboState = ComboState.MAGIC;
            inAttack = true;
            comboTime = playerAnimation.GetCurrentAnimationTime();
        }
    }

    private void SetAttack(AttackData newAttack)
    {
        currentAttack = newAttack;
        playerAnimation.TriggerAnimation(currentAttack.animation);
        inAttack = true;
        attackDuration = currentAttack.duration;
        cancelTime = currentAttack.cancelTime;
        characterController.TransitionToState(CharacterState.Attack);
    }

    private void SetAnimation(string animationName)
    {
        currentAnimation = equippedAbilities.FindAnimation(animationName);
        playerAnimation.TriggerAnimation(currentAnimation.animation);
        inAttack = true;
        attackDuration = currentAnimation.duration;
        cancelTime = currentAnimation.cancelTime;
        characterController.TransitionToState(CharacterState.Attack);
    }

    #endregion

    void ResetComboState() // counts down combo timer, resets combo when time is up
    {
        if (inAttack)
        {
            //Debug.Log(cancelTime + ", " + attackDuration);
            comboTime = playerAnimation.GetCurrentAnimationTime();
            charaMove.canMove = false;
            if (comboTime >= attackDuration)
            {
                currentComboState = ComboState.NONE;
                inAttack = false;
                characterController.TransitionToState(CharacterState.Default);
                charaMove.canMove = true;
                comboChain.ResetComboState();
                comboTime = 1;
                Debug.Log("RESET");
            }
        }
    }
}
