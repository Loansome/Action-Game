using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private CharacterAnimation enemyAnim;
    private Rigidbody enemyBody;
    private Transform playerTarget;

    public float speed = 1.8f;
    public float attackDistance = 1.2f;
    public float movementRange = 8.0f;
    private float chaseAfterAttack = 1f;
    private float currentAttackTime;
    public float defaultAttackTime = 2f;
    private bool following, attacking;
    private bool kissed;

    public Transform attackPoint;
    public Vector3 attackRange;
    public LayerMask playerLayer;

    public int maxHealth = 3;
    int currentHealth;

    private void Awake()
    {
        enemyAnim = GetComponentInChildren<CharacterAnimation>();
        enemyBody = GetComponent<Rigidbody>();
        playerTarget = GameObject.Find("Player").transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentAttackTime = defaultAttackTime;
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, playerTarget.position) < movementRange)
            following = true;
        Attack();
        currentAttackTime += Time.deltaTime;
        if (currentAttackTime > defaultAttackTime && kissed)
        {
            kissed = false;
            currentAttackTime = defaultAttackTime;
            enemyAnim.playIdle();
        }
    }

    private void FixedUpdate()
    {
        FollowTarget();
        if (Vector3.Dot((playerTarget.position - transform.position).normalized, transform.forward) < 0f)
            transform.rotation = Quaternion.LookRotation(-transform.forward, Vector3.up);
    }

    void FollowTarget()
    {
        if (!following || kissed) return;
        if (Vector3.Distance(transform.position, playerTarget.position) > attackDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerTarget.position, speed * Time.deltaTime);

            enemyAnim.Walk(true);
        }
        else if (Vector3.Distance(transform.position, playerTarget.position) <= attackDistance)
        {
            //enemyBody.velocity = Vector3.zero;
            enemyAnim.Walk(false);
            following = false;
            attacking = true;
        }
    }

    void Attack()
    {
        if (!attacking) return;
        if (currentAttackTime > defaultAttackTime)
        {
            enemyAnim.EnemyAttack();
            currentAttackTime = 0f;

            Collider[] hitPlayer = Physics.OverlapBox(attackPoint.position, attackRange, Quaternion.Euler(0,0,0), playerLayer);
            foreach (Collider player in hitPlayer)
            {
                player.GetComponent<PlayerMovement>().TakeDamage(177);
            }
        }
        if (Vector3.Distance(transform.position, playerTarget.position) > attackDistance + chaseAfterAttack)
        {
            attacking = false;
            following = true;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        enemyAnim.Hit();
        if (kissed)
        {
            kissed = false;
            currentAttackTime = 0f;
        }

        if (currentHealth <= 0)
        {
            EnemyDeath();
        }
        else
            FindObjectOfType<AudioManager>().Play("Hit");
    }

    void EnemyDeath()
    {
        Debug.Log("Enemy downed");
        enemyAnim.Death();
        this.enabled = false;
        if (this.name == "Boss")
        {
            FindObjectOfType<AudioManager>().Stop("Boss");
            FindObjectOfType<AudioManager>().Stop("Boss Loop");
            FindObjectOfType<AudioManager>().Play("Enemy Death");
            StartCoroutine(PlayDead());
        }
        else
        {
            FindObjectOfType<AudioManager>().Play("Enemy Death");
            GameObject.Find("Player").GetComponent<PlayerMovement>().enemiesLeft--;
        }
        GetComponent<Collider>().enabled = false;
        if (GameObject.Find("Player").GetComponent<PlayerMovement>().enemiesLeft == 0)
        {
            GameObject.Find("Boss Gate").GetComponent<Collider>().enabled = false;
        }
    }

    IEnumerator PlayDead()
    {
        yield return new WaitForSecondsRealtime(2);
        FindObjectOfType<AudioManager>().Play("Win");
        GameObject.Find("Canvas").GetComponent<PauseMenu>().Win();
        yield return 0;
    }

    public void Kissed()
    {
        if (!kissed)
        {
            enemyAnim.Kissed();
            currentAttackTime -= 5f;
            kissed = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireCube(attackPoint.position, attackRange);
    }
}
