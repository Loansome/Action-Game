using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attack
{
    public bool active = true;

    public float startFrame;
    public float endFrame;

    public float cancelWindowStart;
    public static float whiffCancelWindow = 6f;

    public int hitPause = 3;
    public int hitStun = 30;
    public float damage = 1f;
    public int attackLevel;

    public enum HitDirection { HIGH, MID, STUN }
    public HitDirection hitDirection;

    public enum HitPower { LIGHT, VEL_FACE, ROLL, PARRY }
    public HitPower hitPower;
    public Vector2 knockBack;

    public bool ignoreChainKnockback;
    public enum KnockbackType { FORWARD, RADIAL }
    public KnockbackType knockbackType;

    public enum ImpactType { NOTHING, REFLECT, SLAM }
    public ImpactType impactType;
    public float impactPow = 1f;
    //public Vector3 hitRoll;
    //public Vector3 hitRollFriction;

    public enum HitboxType { ROUNDED, SQUARE, SEMI_CIRCLE, CIRCLE }
    public HitboxType hitboxType;
    public Vector3 hitBoxScale = new Vector3(2f,2f, 3f);
    public Vector3 hitBoxPos = new Vector3(0f,0.2f, 1f);
    public Vector3 hitBoxRot = new Vector3(0f, 0.0f, 0f);

    public bool guardBreak;


    //Collider Info + more complex colliders? Maybe just position, rotation and scale, Circles would probably be good too I think
    //To-Do: have collider velocity to rotate and scale them during the attack
}
