using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "Attack")]
public class AttackData : AbilityData
{
    public enum DamageAttribute { Physical, Fire, Blizzard, Thunder, None}
    public enum KnockbackType { Hit, Finish, Magnetize, Swirl, Launch, FlipOut}
    public float damage;
    public float damageArmor;
    public float damageRevenge;
    public DamageAttribute damageAttribute;
    public KnockbackType knockbackType;
    public bool isCounterAttack;
    public bool isInvincible;
    public bool isGuardable;
    public bool isKillable;


    public float duration;
    public float cancelTime;
    public bool isFinisher;
    public bool isAOE;
    public bool isRanged;
    public Vector2 activationDistance;

    public AnimationSet animation;

}
