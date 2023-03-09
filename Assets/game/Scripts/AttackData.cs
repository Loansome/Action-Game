using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "New Attack Ability")]
public class AttackData : AnimationData
{
    public enum DamageAttribute { Physical, Fire, Blizzard, Thunder, None}
    public enum KnockbackType { Hit, Finish, Magnetize, Swirl, Launch, FlipOut}

    [Header("Attack Info")]
    public float damage;
    public float damageArmor;
    public float damageRevenge;
    public DamageAttribute damageAttribute;
    public KnockbackType knockbackType;
    public bool isCounterAttack;
    public bool isKillable;


    public bool isFinisher;
    //public bool isAerial;
    public bool isAOE;
    public bool isRanged;
    public Vector2 activationDistance;

    public List<Transitions> Transitions;
}
