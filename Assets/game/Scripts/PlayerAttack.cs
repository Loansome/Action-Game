using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;

public enum ComboState
{
    NONE,
    JAB,
    FINISH,
    KISS,
    DODGE,
    MAGIC
}

public class PlayerAttack : MonoBehaviour
{

    private CharacterAnimation playerAnimation;

    private bool inAttack;
    private float defaultComboTimer = 0.7f;
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
        switch (currentComboState)
        {
            default:
            case ComboState.NONE:
                CheckAttack();
                break;
            case ComboState.JAB:
                CheckAttack();
                break;
            case ComboState.FINISH:
                Finish();
                break;
            case ComboState.DODGE:
                CheckAttack();
                break;
            case ComboState.MAGIC:
                Magic();
                break;
        }
        if (currentAttack) ManageEvents();
        ResetComboState();
    }

    public void SetActionState(ComboState newState)
	{
        TrySetState(newState);
	}

    private bool TrySetState(ComboState newState)
	{
        if (newState == currentComboState) return false;
        else
        {
            currentComboState = newState;
            return true;
        }
	}

    private void ManageEvents()
	{
        if (comboTime >= currentAttack.startGravity && comboTime <= currentAttack.endGravity)
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

    private void SetAttackByName(string attackName)
	{
        currentAttack = equippedAbilities.FindAttack(attackName);
        playerAnimation.TriggerAnimation(currentAttack.animation);
        inAttack = true;
        attackDuration = currentAttack.duration;
        //Debug.Log(playerAnimation.FindAnimation(currentAttack.animation));
        //currentComboTimer = 0;
        cancelTime = currentAttack.cancelTime;
        characterController.TransitionToState(CharacterState.Attack);
    }

    private void SetAttack(AttackData newAttack)
    {
        currentAttack = newAttack;
        playerAnimation.TriggerAnimation(currentAttack.animation);
        inAttack = true;
        attackDuration = currentAttack.duration;
        //Debug.Log(playerAnimation.FindAnimation(currentAttack.animation));
        //currentComboTimer = 0;
        cancelTime = currentAttack.cancelTime;
        characterController.TransitionToState(CharacterState.Attack);
    }

    private void SetAnimation(string animationName)
    {
        currentAnimation = equippedAbilities.FindAnimation(animationName);
        playerAnimation.TriggerAnimation(currentAnimation.animation);
        inAttack = true;
        attackDuration = currentAnimation.duration;
        //attackDuration = playerAnimation.FindAnimation(currentAnimation.animation).length;
        //Debug.Log(playerAnimation.FindAnimation(currentAttack.animation));
        cancelTime = currentAnimation.cancelTime;
        characterController.TransitionToState(CharacterState.Attack);
    }

	#region StateMachine

	private void CheckAttack()
	{
        if (inputAttack) // check if player attacks
        {
            Jab();
        }
        else if (inputDodge) // check if player dodges
        {
            Dodge();
        }
    }

    private bool JabConditions()
	{
        if ((currentComboState == ComboState.JAB && cancelTime <= comboTime || currentComboState == ComboState.NONE)) // don't punch if end of combo/kicking
            return true;
        else return false;
    }
    private bool DodgeConditions()
	{
        if (equippedAbilities.IsEquipped("Dodge Roll") && (currentComboState == ComboState.NONE && characterController.IsGrounded()))
            return true;
        else return false;

    }

    private void None() { }

    private void Jab() {
        if (JabConditions()) // check if player attacks
        {
            AttackData newAttack = comboChain.TryNextAttack(!characterController.IsGrounded());
            if (newAttack != null)
            {
                currentAttack = newAttack;
                comboChain.SetCurrentAttack(newAttack);
                SetAttack(newAttack);
                playerAnimation.Attack();
                Debug.Log("Jab");
                TrySetState(ComboState.JAB);
            }

            /*Collider[] hitEnemies = Physics.OverlapBox(attackPoint.position, attackRange, Quaternion.Euler(0,0,0), enemyLayers);
            foreach (Collider enemy in hitEnemies)
            {
                enemy.GetComponent<EnemyMovement>().TakeDamage(1);
            }*/
        }
    }

    private void Finish() { }

    private void Dodge() {
        if (DodgeConditions()) // check if player dodges
        {
            SetAnimation("Dodge Roll");
            playerAnimation.Guard();
            Debug.Log("Dodge");
            TrySetState(ComboState.DODGE);
        }
    }
    private void Magic() {
        if (inputMagic)
        {
            if (currentComboState != ComboState.NONE)
                return;

            currentComboState++;
            inAttack = true;
            comboTime = defaultComboTimer + .1f;

            currentComboState = ComboState.MAGIC;
            playerAnimation.Magic();
        }
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

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireCube(attackPoint.position, attackRange);
    }
}
