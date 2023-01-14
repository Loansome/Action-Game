using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationSet
{
    Idle,
    Run,
    GroundAttack1,
    DodgeRoll,
}

public class CharacterAnimation : MonoBehaviour
{

    private Animator anim;
    private int animLayer = 0;
    private AnimationSet currentAnimation = AnimationSet.Run;
    private bool isTriggeredThisFrame = false;
    
    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    #region Animations
    public static readonly int Idle = Animator.StringToHash("Idle");
    public static readonly int Run = Animator.StringToHash("Run");
    public static readonly int Attack1 = Animator.StringToHash("GroundAttack1");
    public static readonly int Dodge = Animator.StringToHash("DodgeRoll");

    private int setAnimationFromEnum(AnimationSet animstate)
    {
        switch (animstate)
        {
            case AnimationSet.Idle:
                {
                    currentAnimation = AnimationSet.Idle;
                    return Idle;
                }
            case AnimationSet.Run:
                {
                    currentAnimation = AnimationSet.Run;
                    return Run;
				}
            case AnimationSet.GroundAttack1:
				{
                    currentAnimation = AnimationSet.GroundAttack1;
                    return Attack1;
				}
            case AnimationSet.DodgeRoll:
                {
                    currentAnimation = AnimationSet.DodgeRoll;
                    return Dodge;
                }
        }
        return 0;
    }


    private void Update()
	{
        if (isTriggeredThisFrame == false && (anim.GetCurrentAnimatorStateInfo(animLayer).shortNameHash == Idle || anim.GetCurrentAnimatorStateInfo(animLayer).shortNameHash == Run))
            currentAnimation = AnimationSet.Idle;
        isTriggeredThisFrame = false;
        //Debug.Log(currentAnimation);
	}
    public void TriggerAnimation(AnimationSet animation)
	{
        anim.CrossFade(setAnimationFromEnum(animation), 0, animLayer);
        isTriggeredThisFrame = true;
	}
	#endregion

	#region Triggers
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
        //anim.SetTrigger("Attack");
    }
    public void Guard()
    {
        //anim.SetTrigger("Guard");
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
	#endregion

    public AnimationSet getAnimationSet()
	{
        return currentAnimation;
	}

    public float getRootCurve()
	{
        return anim.GetFloat("RootForward");
	}

	public bool isPlaying(string stateName)
    {
        if (getCurrentAnimation().name == stateName &&
                anim.GetCurrentAnimatorStateInfo(animLayer).normalizedTime < 1.0f)
            return true;
        else
            return false;
    }

    public AnimationClip getCurrentAnimation()
	{
        return anim.GetCurrentAnimatorClipInfo(animLayer)[0].clip;
	}

}
