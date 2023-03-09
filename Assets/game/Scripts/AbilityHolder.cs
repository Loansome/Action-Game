using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AbilityHolder : MonoBehaviour
{
	public List<Ability> abilities = new List<Ability>();
	//public Dictionary<enum?, string> abilityName; // create list to better organize names, making sure there are no mispells

	public void LockAbility(string name)
	{
		var ability = FindAbility(name).abilityState;
		ability = AbilityState.Locked;
	}
	public void FreeAbility(string name)
	{
		var ability = FindAbility(name).abilityState;
		ability = AbilityState.Free;
	}
	public void EquipAbility(string name)
	{
		var ability = FindAbility(name).abilityState;
		ability = AbilityState.Equipped;
	}

	public bool IsEquipped(string name)
	{
		var abilityState = FindAbility(name).abilityState;
		return abilityState == AbilityState.Equipped || abilityState == AbilityState.LockEquipped;
	}

	public int CheckAmountEquipped(string name)
	{
		int amountEquipped = 0;
		foreach (Ability ability in abilities)
		{
			if (ability.abilityData.name == name && (ability.abilityState == AbilityState.Equipped || ability.abilityState == AbilityState.LockEquipped))
			{
				amountEquipped++;
			}
		}

		return amountEquipped;
	}

	public Ability FindAbility(string name)
	{
		return abilities.Find(t => t.abilityData.name == name);
	}

	public AnimationData FindAnimation(string name)
	{
		return (AnimationData)FindAbility(name).abilityData;
	}

	public AttackData FindAttack(string name)
	{
		return (AttackData)FindAbility(name).abilityData;
	}

	private void OnDrawGizmosSelected()
	{
		foreach (var item in abilities)
		{
			AttackData currentAbility = (AttackData)item.abilityData;
			if (currentAbility != null)
			{
				Gizmos.color = Color.yellow;
				Gizmos.DrawWireSphere(transform.position, currentAbility.activationDistance.x);
				Gizmos.color = Color.cyan;
				Gizmos.DrawWireSphere(transform.position, currentAbility.activationDistance.y);
			}
		}
	}
}
