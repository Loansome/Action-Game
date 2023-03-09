using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

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
    public static readonly string Idle = "Idle";
    public static readonly string Run = ("Run");
    public static readonly string OverheadAttack = ("GroundAttack1");
    public static readonly string SidewaysAttack = ("GroundAttack2");
    public static readonly string GroundFarAttack = ("GroundFarAttack");
    public static readonly string OverheadFinisher = ("GroundFinisher1");
    public static readonly string SideFinisher = ("GroundFinisher2");
    public static readonly string DownAirAttack = ("AirAttack1");
    public static readonly string UpAirAttack = ("AirAttack2");
    public static readonly string DownAirFinish = ("AirFinisher1");
    public static readonly string SideAirFinish = ("AirFinisher2");
    public static readonly string Block = ("Guard");
    public static readonly string Dodge = ("DodgeRoll");

    private string SetAnimationFromEnum(AnimationSet animstate)
    {
        switch (animstate)
        {
            case AnimationSet.Idle:
                {
                    //currentAnimation = AnimationSet.Idle;
                    return Idle;
                }
            case AnimationSet.Run:
                {
                    //currentAnimation = AnimationSet.Run;
                    return Run;
				}
            case AnimationSet.OverheadAttack:
				{
                    //currentAnimation = AnimationSet.OverheadAttack;
                    return OverheadAttack;
                }
            case AnimationSet.SidewaysAttack:
                {
                    //currentAnimation = AnimationSet.SidewaysAttack;
                    return SidewaysAttack;
                }
            case AnimationSet.GroundFarAttack:
                {
                    //currentAnimation = AnimationSet.GroundFarAttack;
                    return GroundFarAttack;
                }
            case AnimationSet.OverheadFinisher:
                {
                    //currentAnimation = AnimationSet.OverheadFinisher;
                    return OverheadFinisher;
                }
            case AnimationSet.SideFinisher:
                {
                    //currentAnimation = AnimationSet.SideFinisher;
                    return SideFinisher;
                }
            case AnimationSet.DownAirAttack:
                {
                    //currentAnimation = AnimationSet.DownAirAttack;
                    return DownAirAttack;
                }
            case AnimationSet.UpAirAttack:
                {
                    //currentAnimation = AnimationSet.UpAirAttack;
                    return UpAirAttack;
                }
            case AnimationSet.DownAirFinish:
                {
                    //currentAnimation = AnimationSet.DownAirFinish;
                    return DownAirFinish;
                }
            case AnimationSet.SideAirFinish:
                {
                    //currentAnimation = AnimationSet.SideAirFinish;
                    return SideAirFinish;
                }
            case AnimationSet.Block:
                {
                    //currentAnimation = AnimationSet.Block;
                    return Block;
                }
            case AnimationSet.DodgeRoll:
                {
                    //currentAnimation = AnimationSet.DodgeRoll;
                    return Dodge;
                }
        }
        return "";
    }


    private void Update()
	{
        if (isTriggeredThisFrame == false && (anim.GetCurrentAnimatorClipInfo(animLayer)[0].clip.name == Idle || anim.GetCurrentAnimatorClipInfo(animLayer)[0].clip.name == Run))
            currentAnimation = AnimationSet.Idle;
        isTriggeredThisFrame = false;
        //Debug.Log(anim.GetCurrentAnimatorClipInfo(animLayer)[0].clip.name);
	}
    public void TriggerAnimation(AnimationSet animation)
	{
        anim.CrossFade(SetAnimationFromEnum(animation), 0, animLayer);
        currentAnimation = animation;
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

    public AnimationSet GetAnimationSet()
	{
        return currentAnimation;
	}

    public float GetRootCurve()
	{
        return anim.GetFloat("RootForward");
	}

	public bool IsPlaying(string stateName)
    {
        if (GetCurrentAnimation().name == stateName &&
                anim.GetCurrentAnimatorStateInfo(animLayer).normalizedTime < 1.0f)
            return true;
        else
            return false;
    }

    public AnimationClip GetCurrentAnimation()
	{
        return anim.GetCurrentAnimatorClipInfo(animLayer)[0].clip;
	}
    public AnimationClip FindAnimation(AnimationSet animation)
	{
        string findAnim = "Rig|" + SetAnimationFromEnum(animation);
        //Debug.Log(findAnim);

        // Get the controller that the Animator is using
        AnimatorController animController = anim.runtimeAnimatorController as AnimatorController;

        // Get the attack animation clip from the controller
        foreach (AnimationClip clip in animController.animationClips)
        {
            //Debug.Log(clip.name + ",  " + findAnim);
            if (clip.name == findAnim)
            {
                return clip;
            }
        }
        return null;
	}

    public float GetCurrentAnimationTime()
	{
        return anim.GetCurrentAnimatorStateInfo(animLayer).normalizedTime;
    }
}
