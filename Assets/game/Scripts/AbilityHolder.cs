using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class AbilityHolder : MonoBehaviour
{
	public List<Ability> abilities = new List<Ability>();
	[SerializeField] private ComboChainHandler comboChain;
	//public Dictionary<enum?, string> abilityName; // create list to better organize names, making sure there are no mispells

	private void Start()
	{
		
	}
	private void OnValidate()
	{
		comboChain.UpdateComboAbilities();
	}

	public void LockAbility(string name)
	{
		var ability = FindAbilityByName(name).abilityState;
		ability = AbilityState.Locked;
	}
	public void FreeAbility(string name)
	{
		var ability = FindAbilityByName(name).abilityState;
		ability = AbilityState.Free;
	}
	public void EquipAbility(string name)
	{
		var ability = FindAbilityByName(name).abilityState;
		ability = AbilityState.Equipped;
	}

	public bool IsEquipped(string name)
	{
		var abilityState = FindAbilityByName(name).abilityState;
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

	public Ability FindAbilityByName(string name)
	{
		return abilities.Find(t => t.abilityData.name == name);
	}

	public AnimationData FindAnimation(string name)
	{
		return (AnimationData)FindAbilityByName(name).abilityData;
	}

	public AttackData FindAttack(string name)
	{
		return (AttackData)FindAbilityByName(name).abilityData;
	}

	private void OnDrawGizmos()
	{
		foreach (var item in abilities)
		{
			AttackData currentAbility = null;
			try
			{
				currentAbility = (AttackData)item.abilityData;
			} catch
			{
				currentAbility = null;
			}
			if (currentAbility != null)
			{
				Handles.color = Color.green;
				Handles.DrawWireDisc(transform.position, transform.up, currentAbility.activationDistance.x);
				Handles.color = Color.yellow;
				Handles.DrawWireDisc(transform.position, transform.up, currentAbility.activationDistance.y);
			}
		}
	}
}
