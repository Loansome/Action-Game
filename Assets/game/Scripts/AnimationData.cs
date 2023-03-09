using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Animation", menuName = "New Animation Ability")]
public class AnimationData : AbilityData
{
    [Header("Animation Info")]
    [Range(0f, 1f)] public float duration;
    [Range(0f, 1f)] public float cancelTime;
    public bool isInvincible;
    public bool isGuardable;

    public AnimationSet animation;

    public List<AttackEvents> attackEvents;

    public AnimationCurve forwardMove;
    public AnimationCurve upwardMove;
}
