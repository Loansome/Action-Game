using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationSet
{
    Idle,
    Run,
    OverheadAttack, //overhead
    SidewaysAttack, // sideways swing
    GroundFarAttack,
    OverheadFinisher,
    SideFinisher,
    DownAirAttack,
    UpAirAttack,
    DownAirFinish,
    SideAirFinish,
    Block,
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
    public static readonly int OverheadAttack = Animator.StringToHash("GroundAttack1");
    public static readonly int SidewaysAttack = Animator.StringToHash("GroundAttack2");
    public static readonly int GroundFarAttack = Animator.StringToHash("GroundFarAttack");
    public static readonly int OverheadFinisher = Animator.StringToHash("GroundFinisher1");
    public static readonly int SideFinisher = Animator.StringToHash("GroundFinisher2");
    public static readonly int DownAirAttack = Animator.StringToHash("AirAttack1");
    public static readonly int UpAirAttack = Animator.StringToHash("AirAttack2");
    public static readonly int DownAirFinish = Animator.StringToHash("AirFinisher1");
    public static readonly int SideAirFinish = Animator.StringToHash("AirFinisher2");
    public static readonly int Block = Animator.StringToHash("Guard");
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
            case AnimationSet.OverheadAttack:
				{
                    currentAnimation = AnimationSet.OverheadAttack;
                    return OverheadAttack;
                }
            case AnimationSet.SidewaysAttack:
                {
                    currentAnimation = AnimationSet.SidewaysAttack;
                    return SidewaysAttack;
                }
            case AnimationSet.GroundFarAttack:
                {
                    currentAnimation = AnimationSet.GroundFarAttack;
                    return GroundFarAttack;
                }
            case AnimationSet.OverheadFinisher:
                {
                    currentAnimation = AnimationSet.OverheadFinisher;
                    return OverheadFinisher;
                }
            case AnimationSet.SideFinisher:
                {
                    currentAnimation = AnimationSet.SideFinisher;
                    return SideFinisher;
                }
            case AnimationSet.DownAirAttack:
                {
                    currentAnimation = AnimationSet.DownAirAttack;
                    return DownAirAttack;
                }
            case AnimationSet.UpAirAttack:
                {
                    currentAnimation = AnimationSet.UpAirAttack;
                    return UpAirAttack;
                }
            case AnimationSet.DownAirFinish:
                {
                    currentAnimation = AnimationSet.DownAirFinish;
                    return DownAirFinish;
                }
            case AnimationSet.SideAirFinish:
                {
                    currentAnimation = AnimationSet.SideAirFinish;
                    return SideAirFinish;
                }
            case AnimationSet.Block:
                {
                    currentAnimation = AnimationSet.Block;
                    return Block;
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
