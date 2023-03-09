using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class HitEffectManager : ScriptableObject
{
    public State genericHitstun;

    public State genericGuard;

    public State genericGuardCancel;

    public State genericGuardBreak;

    public GameObject guardBreakVFXPrefab;
}

