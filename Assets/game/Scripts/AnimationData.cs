using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Animation", menuName = "New Animation Ability")]
public class AnimationData : AbilityData
{
    [Header("Animation Info")]
    public bool isAerial = false;
    [Range(0f, 1f)] public float duration;
    [Range(0f, 1f)] public float cancelTime;
    [Range(0f, 1f)] public float gravityMod = 1;
    [Range(0f, 1f)] public float startGravity;
    [Range(0f, 1f)] public float endGravity;
    public bool isInvincible;
    public bool isGuardable;

    public AnimationSet animation;

    public AnimationCurve forwardMove;
    public AnimationCurve upwardMove;
}
