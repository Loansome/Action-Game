using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackContextHandler : MonoBehaviour
{
    AbilityHolder heldAbilities;

    // Start is called before the first frame update
    void Start()
    {
        heldAbilities = GetComponent<AbilityHolder>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public AttackData FetchNextAttack(AttackData lastAttack, bool checkFinisher, bool checkAerial)
	{
        List<AttackData> equippedAttacks = new List<AttackData>(CheckAvailableAttacks(checkFinisher, checkAerial));
        if (lastAttack != null)
        {
            foreach (Transitions attack in lastAttack.Transitions)
            {
                if (equippedAttacks.Contains(attack.Attack))
                {
                    Debug.Log(attack.Attack);
                    return attack.Attack;
				}
            }
        }
        //Debug.Log(equippedAttacks.Count);
        return equippedAttacks[0];
	}

    private AttackData[] CheckAvailableAttacks(bool checkFinisher, bool checkAerial)
    {
        List<AttackData> availableAttacks = new List<AttackData>();
        AttackData attack = null;
        foreach (Ability ability in heldAbilities.abilities)
        {
            try
            {
                attack = (AttackData)ability.abilityData;
            }
            catch
            {
                attack = null;
            }
            // checks if the ability is an attack, if it's equipped, and if it's a finisher or aerial attack
            if (attack != null)
            {

                if ((ability.abilityState == AbilityState.Equipped || ability.abilityState == AbilityState.LockEquipped)
                    && attack.isFinisher == checkFinisher
                    && attack.isAerial == checkAerial)
                {
                    availableAttacks.Add(attack); // if it lines up, add it to the list
                    //Debug.Log("Added " + attack.name);
                }
            }
        }
        return availableAttacks.ToArray();
    }
}
