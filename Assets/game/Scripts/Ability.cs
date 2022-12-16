using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

	public enum AbilityState { Locked, LockEquipped, Free, Equipped };
[Serializable]
public struct Ability
{
	public AbilityData abilityData;
	public AbilityState abilityState;
}
