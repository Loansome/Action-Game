using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{

    private Animator anim;
    private int animLayer = 0;
    
    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void Walk(bool move)
    {
        anim.SetBool("Walk", move);
    }
    public void Jump(bool jump)
    {
        anim.SetBool("Jump", jump);
    }
    public void IsGrounded(bool isGrounded)
    {
        anim.SetBool("IsGrounded", isGrounded);
    }
    public void Land()
    {
        anim.SetTrigger("Land");
    }
    public void Attack()
    {
        anim.SetTrigger("Attack");
    }
    public void Guard()
    {
        anim.SetTrigger("Guard");
    }
    public void Magic()
    {
        anim.SetTrigger("Magic");
    }

    public void Kiss()
    {
        anim.SetTrigger("Kiss");
    }

    public void EnemyAttack()
    {
        //if (attack == 0)
            anim.SetTrigger("Attack");
    }
    public void playIdle()
    {
        anim.Play("Idle");
    }
    public void Hit()
    {
        anim.SetTrigger("Hit");
    }
    public void Death()
    {
        anim.SetTrigger("Death");
    }
    public void Kissed()
    {
        anim.SetTrigger("Kissed");
    }

    public bool isPlaying(string stateName)
    {
        if (anim.GetCurrentAnimatorStateInfo(animLayer).IsName(stateName) &&
                anim.GetCurrentAnimatorStateInfo(animLayer).normalizedTime < 1.0f)
            return true;
        else
            return false;
    }

}
