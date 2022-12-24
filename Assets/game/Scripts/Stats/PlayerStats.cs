using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : EntityStats
{
    [Header("PlayerStats")]
    [SerializeField] private float _maxMagic;
    [SerializeField] private float _currentMagic;
    [SerializeField] private int _maxDrive;
    [SerializeField] private float _currentDrive;

    // Start is called before the first frame update
    void Start()
    {
        _currentMagic = _maxMagic;
        _currentDrive = _maxDrive;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeMagic(float amount)
	{
        _currentMagic -= amount;
        if (_currentMagic <= 0)
		{
            // recharge
		}
	}
    public void GiveMagic(float amount)
	{
        _currentMagic += amount;
        if (_currentMagic > _maxMagic)
		{
            _currentMagic = _maxMagic;
		}
	}

    public void TakeDrive(float amount)
	{
        _currentDrive -= amount;
        if (_currentDrive <= 0)
        {
            _currentDrive = 0;
        }
    }
    public void GiveDrive(float amount)
    {
        _currentDrive += amount;
        if (_currentDrive > _maxDrive)
        {
            _currentDrive = _maxDrive;
        }
    }
}
