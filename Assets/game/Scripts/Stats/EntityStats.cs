using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType
{
    Human,
    Heartless,
    Nobody,
}

public enum ElementType
{
    Normal,
    Fire,
    Ice,
    Thunder,
    Wind,
    Flying,
    Earth,
    Light,
    Dark,
}

public class EntityStats : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] private float _maxHealth = 100;
    [SerializeField] private float _currentHealth;
    [SerializeField] private float _attackPower = 10;
    [SerializeField] private float _defensePower = 10;
    [SerializeField] private EntityType _entityType;
    [SerializeField] private ElementType _elementType;
    [SerializeField] private float[] _elementResistance = new float[9]; // set to the size of existing elements pls
    [SerializeField] private float[] _elementWeakness = new float[9]; // set to the size of existing elements pls


    // Start is called before the first frame update
    void Start()
    {
        _currentHealth = _maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage)
	{
        _currentHealth -= damage;
        if (_currentHealth <= 0)
		{
            _currentHealth = 0;
            //die
		}
	}

    public void Heal(float amount)
	{
        _currentHealth += amount;
        if (_currentHealth > _maxHealth)
		{
            _currentHealth = _maxHealth;
		}
	}
}
