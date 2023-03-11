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
    AerialFinish,
}

public class CharacterAnimation : MonoBehaviour
{

    private Animator anim;
    private int animLayer = 0;
    
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
    public static readonly string OverheadFinisher = ("GroundFinisher");
    public static readonly string SideFinisher = ("GroundFinisher2");
    public static readonly string DownAirAttack = ("AirAttack1");
    public static readonly string UpAirAttack = ("AirAttack2");
    public static readonly string DownAirFinish = ("AirFinisher1");
    public static readonly string SideAirFinish = ("AirFinisher2");
    public static readonly string Block = ("Guard");
    public static readonly string Dodge = ("DodgeRoll");
    public static readonly string AerialFinish = ("AerialFinish");

    private string SetAnimationFromEnum(AnimationSet animstate)
    {
        switch (animstate)
        {
            case AnimationSet.Idle:
                {
                    return Idle;
                }
            case AnimationSet.Run:
                {
                    return Run;
				}
            case AnimationSet.OverheadAttack:
				{
                    return OverheadAttack;
                }
            case AnimationSet.SidewaysAttack:
                {
                    return SidewaysAttack;
                }
            case AnimationSet.GroundFarAttack:
                {
                    return GroundFarAttack;
                }
            case AnimationSet.OverheadFinisher:
                {
                    return OverheadFinisher;
                }
            case AnimationSet.SideFinisher:
                {
                    return SideFinisher;
                }
            case AnimationSet.DownAirAttack:
                {
                    return DownAirAttack;
                }
            case AnimationSet.UpAirAttack:
                {
                    return UpAirAttack;
                }
            case AnimationSet.DownAirFinish:
                {
                    return DownAirFinish;
                }
            case AnimationSet.SideAirFinish:
                {
                    return SideAirFinish;
                }
            case AnimationSet.Block:
                {
                    return Block;
                }
            case AnimationSet.DodgeRoll:
                {
                    return Dodge;
                }
            case AnimationSet.AerialFinish:
                {
                    return AerialFinish;
                }
        }
        return "";
    }

    public void Jump(bool didJump)
	{
        anim.SetBool("Jump", didJump);
	}
    public void IsGrounded(bool isGrounded)
    {
        anim.SetBool("IsGrounded", isGrounded);
    }
    public void Walk(bool isWalking)
    {
        anim.SetBool("Walk", isWalking);
    }

    #endregion

    public void TriggerAnimation(AnimationSet animation)
	{
        anim.CrossFade(SetAnimationFromEnum(animation), 0, animLayer);
	}

    public float GetForwardCurve()
	{
        return anim.GetFloat("RootForward");
	}
    public float GetUpwardCurve()
    {
        return anim.GetFloat("RootUp");
    }

    public float GetCurrentAnimationTime()
	{
        return anim.GetCurrentAnimatorStateInfo(animLayer).normalizedTime;
    }
}
