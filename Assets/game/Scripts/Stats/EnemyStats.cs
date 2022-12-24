using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : EntityStats
{
    [Header("Enemy Stats")]
    [SerializeField] private float _maxArmorHealth = 0;
    [SerializeField] private float _currentArmorHealth;
    [SerializeField] private float _revengeValueLimit = 7;
    [SerializeField] private float _currentRevengeValue;
    [SerializeField] private float _revengeCooldownSpeed = 5;
    [SerializeField] private int[] _dropPrize;
    [SerializeField] private int[] _dropItem;

    // Start is called before the first frame update
    void Start()
    {
        _currentArmorHealth = _maxArmorHealth;
        _currentRevengeValue = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*public override void TakeDamage(float damage)
	{
        if (_currentArmorHealth >= 0)
		{

		}
	}*/

    public void TakeRevengeValue(int amount)
	{
        _currentRevengeValue += amount;
        if (_currentRevengeValue >= _revengeValueLimit)
		{
            // retaliate, possibly start cooldown (after a bit, prob managed in there)
		}
	}
    private void CoolRevengeValue()
	{

	}
}
