using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private CharacterAnimation playerAnimation;
    private Rigidbody playerBody;

    public float walkSpeed = 2.5f;
    public float zSpeed = 5f;

    public int enemiesLeft = 6;

    public int maxHealth = 1000;
    int currentHealth;
    private HealthUI healthUI;

    void Awake()
    {
        playerAnimation = GetComponentInChildren<CharacterAnimation>();
        playerBody = GetComponent<Rigidbody>();
        healthUI = GameObject.FindWithTag("HealthUI").GetComponent<HealthUI>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (!PauseMenu.isMenued)
        {
            RotatePlayer();
            WalkAnim();
        }
    }

    void FixedUpdate()
    {
        if (!PauseMenu.isMenued)
        {
            DetectMovement();
        }
    }

    void DetectMovement() // Gets inputs for movement
    {
        playerBody.velocity = new Vector3(
            Input.GetAxisRaw("Horizontal") * (-walkSpeed),
            playerBody.velocity.y,
            Input.GetAxisRaw("Vertical") * (-zSpeed));
    }
    void RotatePlayer() // Rotates player to face where they're moving
    {
        if (Input.GetAxisRaw("Horizontal") > 0)
            transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        else if (Input.GetAxisRaw("Horizontal") < 0)
            transform.rotation = Quaternion.Euler(0f, 90f, 0f);
    }
    void WalkAnim()
    {
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            playerAnimation.Walk(true);
        else playerAnimation.Walk(false);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Player hit");
        playerAnimation.Hit();
        FindObjectOfType<AudioManager>().Play("Hit");
        healthUI.DisplayHealth(currentHealth);
        if (currentHealth <= 0)
        {
            PlayerDeath();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        FindObjectOfType<AudioManager>().Stop("Scroll 1");
        FindObjectOfType<AudioManager>().Play("Boss");
        GameObject.Find("Boss").GetComponent<EnemyMovement>().movementRange = 10f;
        GameObject.Find("Boss Trigger").SetActive(false);
        GameObject.Find("Boss Gate").GetComponent<Collider>().enabled = true;
        Camera.main.GetComponent<CameraScroll>().xMax = -21;
        //GameObject.Find("Canvas").GetComponent<PauseMenu>().Win();
    }

    void PlayerDeath()
    {
        Debug.Log("Player downed");
        playerAnimation.Death();
        FindObjectOfType<AudioManager>().Stop("Scroll 1");
        FindObjectOfType<AudioManager>().Stop("Boss");
        FindObjectOfType<AudioManager>().Stop("Boss Loop");
        FindObjectOfType<AudioManager>().Play("Player Death");
        this.enabled = false;
        GetComponent<PlayerAttack>().enabled = false;
        GetComponent<Collider>().enabled = false;
        StartCoroutine(PlayDead());
    }

    IEnumerator PlayDead()
    {
        yield return new WaitForSecondsRealtime(2);
        FindObjectOfType<AudioManager>().Play("Game Over");
        GameObject.Find("Canvas").GetComponent<PauseMenu>().GameOver();
        yield return 0;
    }
}
