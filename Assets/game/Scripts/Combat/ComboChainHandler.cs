using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboChainHandler : MonoBehaviour
{
    public int baseAttackLength = 2;
    public int baseFinisherLength = 1;
    public int attackLength;
    public int finisherLength;
    public int fullComboLength;
    public int currentComboPosition = 0;

    public AttackData prevAttack;
    public AttackData currentAttack;

    private AbilityHolder heldAbilities;

    // Start is called before the first frame update
    void Start()
    {
        heldAbilities = GetComponent<AbilityHolder>();

        UpdateComboAbilities();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool TryCurrentAttack(string attackName)
    {
        // check if attack is equipped, then set it. if not, skip
        AttackData newAttack = heldAbilities.FindAttack(attackName);
        if (newAttack == null) return false;

        // check if attack isn't the same as the current one and there's room to continue
        if (currentAttack != newAttack)
        {
            if (!newAttack.isFinisher && currentComboPosition < attackLength && currentComboPosition < fullComboLength)
            {
                return true;
            }
            else if (newAttack.isFinisher && currentComboPosition < finisherLength && currentComboPosition < fullComboLength)
            {
                return true;
            }
        }
        return false;
    }

    public void SetCurrentAttack(string attackName)
    {
        AttackData newAttack = heldAbilities.FindAttack(attackName);
        prevAttack = currentAttack;
        currentAttack = newAttack;
        currentComboPosition++;
        Debug.Log("Set new attack. Combo chain: " + currentComboPosition);
    }

    public void ResetComboState()
	{
        currentAttack = null;
        currentComboPosition = 0;
        Debug.Log("Combo chain: " + currentComboPosition);
	}

    public void UpdateComboAbilities()
	{
        attackLength = baseAttackLength + heldAbilities.CheckAmountEquipped("Combo Plus");
        attackLength -= heldAbilities.CheckAmountEquipped("Negative Combo");
        finisherLength = baseFinisherLength + heldAbilities.CheckAmountEquipped("Finishing Plus");

        fullComboLength = attackLength + finisherLength;
    }
}
