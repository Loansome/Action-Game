using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private bool activateTimerToReset;
    private float defaultComboTimer = 0.7f;
    private float currentComboTimer;
    private float cancelComboTimer;
    public ComboState currentComboState;

    public CharacterMovement charaMove;

    public Transform attackPoint;
    public Vector3 attackRange;
    public LayerMask enemyLayers;

    [Header("Actions")]
    public AbilityHolder equippedAbilities;
    public AttackData currentAttack;

    public int jabLength = 2;
    public int finisherLength = 1;
    public int comboLength;
    public int currentComboLength;

    void Awake()
    {
        playerAnimation = GetComponentInChildren<CharacterAnimation>();
        charaMove = GetComponent<CharacterMovement>();
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
                Jab();
                break;
            case ComboState.FINISH:
                Finish();
                break;
            case ComboState.DODGE:
                Dodge();
                break;
            case ComboState.MAGIC:
                Magic();
                break;
        }
        //AttackCombo();
        ResetComboState();
    }

    void AttackCombo()
    {
        if (Input.GetButtonDown("Fire1")) // punch
        {
            if (currentComboState == ComboState.JAB && cancelComboTimer > 0 || currentComboState == ComboState.DODGE || currentComboState == ComboState.MAGIC) // don't punch if end of combo/kicking
                return;

            currentComboState = ComboState.JAB;
            SetAttack("Attack");

            playerAnimation.Attack();
            if (!charaMove.isGrounded)
            {  
                charaMove.AirAttackJump(1f, false);
            }
            Debug.Log("Jab");

            /*Collider[] hitEnemies = Physics.OverlapBox(attackPoint.position, attackRange, Quaternion.Euler(0,0,0), enemyLayers);
            foreach (Collider enemy in hitEnemies)
            {
                enemy.GetComponent<EnemyMovement>().TakeDamage(1);
            }*/
        }

        if (Input.GetButtonDown("Fire2") && charaMove.isGrounded)
        {
            if (currentComboState != ComboState.NONE)
                return;

            currentComboState = ComboState.DODGE;
            currentAttack = equippedAbilities.GetAttackData("Dodge");
            activateTimerToReset = true;
            currentComboTimer = currentAttack.duration;
            cancelComboTimer = currentAttack.cancelTime;

            playerAnimation.Guard();
            Debug.Log("Dodge");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (currentComboState != ComboState.NONE)
                return;

            currentComboState++;
            activateTimerToReset = true;
            currentComboTimer = defaultComboTimer + .1f;

            currentComboState = ComboState.MAGIC;
            playerAnimation.Magic();
        }

        if (Input.GetKeyDown(KeyCode.K)) // kick
        {
            if (currentComboState == ComboState.FINISH || currentComboState == ComboState.KISS || currentComboTimer > 0.35f) // don't kick if ended combo
                return;

            if (currentComboState == ComboState.FINISH)
                currentComboState++;
            else currentComboState = ComboState.FINISH;

            activateTimerToReset = true;
            currentComboTimer = defaultComboTimer;

            Debug.Log("Finish");

            //if (currentComboState == ComboState.KICK)
                //playerAnimation.Kick();

            /*Collider[] hitEnemies = Physics.OverlapBox(attackPoint.position, attackRange, Quaternion.Euler(0, 0, 0), enemyLayers);
            foreach (Collider enemy in hitEnemies)
            {
                enemy.GetComponent<EnemyMovement>().TakeDamage(1);
            }*/
        }

        if (Input.GetKeyDown(KeyCode.L)) // kiss
        {
            if (currentComboState == ComboState.KISS || currentComboTimer > 0.35f) // don't kiss if ended combo
                return;

            currentComboState = ComboState.KISS;

            activateTimerToReset = true;
            currentComboTimer = defaultComboTimer + .2f;

            //playerAnimation.Kiss();

            /*Collider[] hitEnemies = Physics.OverlapBox(attackPoint.position, attackRange, Quaternion.Euler(0, 0, 0), enemyLayers);
            foreach (Collider enemy in hitEnemies)
            {
                enemy.GetComponent<EnemyMovement>().Kissed();
            }*/
        }
    }
    private void SetAttack(string attackName)
	{
        currentAttack = equippedAbilities.GetAttackData(attackName);
        activateTimerToReset = true;
        currentComboTimer = currentAttack.duration;
        cancelComboTimer = currentAttack.cancelTime;
    }

	#region StateMachine

	private void CheckAttack()
	{
        if (Input.GetButtonDown("Fire1") && JabConditions()) // check if player attacks
        {
            SetAttack("Attack");
            playerAnimation.Attack();
            if (!charaMove.isGrounded)
            {
                charaMove.AirAttackJump(1f, false);
            }
            Debug.Log("Jab");
            TrySetState(ComboState.JAB);
            return;
        }
        else if (Input.GetButtonDown("Fire2") && DodgeConditions()) // check if player dodges
        {
            SetAttack("Dodge");
            playerAnimation.Guard();
            Debug.Log("Dodge");
            TrySetState(ComboState.DODGE);
        }
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

    private bool JabConditions()
	{
        if (equippedAbilities.IsEquipped("Attack") && (currentComboState == ComboState.JAB && cancelComboTimer < 0 || currentComboState == ComboState.NONE)) // don't punch if end of combo/kicking
            return true;
        else return false;
    }
    private bool DodgeConditions()
	{
        if (equippedAbilities.IsEquipped("Dodge") && (currentComboState == ComboState.NONE && charaMove.isGrounded))
            return true;
        else return false;

    }
    void Jab() { CheckAttack(); }
    void Finish() { }
    void Dodge() { CheckAttack(); }
    void Magic() { }

	#endregion

	void ResetComboState() // counts down combo timer, resets combo when time is up
    {
        if (activateTimerToReset)
        {
            currentComboTimer -= Time.deltaTime;
            cancelComboTimer -= Time.deltaTime;
            charaMove.isAttacking = true;
            if (currentComboTimer <= 0f)
            {
                currentComboState = ComboState.NONE;
                activateTimerToReset = false;
                charaMove.isAttacking = false;
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
