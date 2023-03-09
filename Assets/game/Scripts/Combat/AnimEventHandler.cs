using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;

public class AnimEventHandler : MonoBehaviour
{
    [SerializeField] private MyCharacterController playerController;
    private float _gravityPrevious;
    private float _gravityDefault = 1;

    // Start is called before the first frame update
    void Start()
    {
        _gravityPrevious = playerController.GravityMod;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Cancelable()
	{
        Debug.Log("Cancel");
	}
    public void DamageStart()
	{
        Debug.Log("Can damage");
	}
    public void DamageEnd()
	{
        Debug.Log("End damage");
    }
    public void GravityChange(float gravityModifier)
	{
        _gravityPrevious = playerController.GravityMod;
        playerController.GravityMod = gravityModifier;
        Debug.Log("Set new gravity");
	}
    public void GravityReset()
	{
        playerController.GravityMod = _gravityDefault; //_gravityPrevious; //_gravityDefault;
        Debug.Log("Reset gravity");
	}
    public void InvincibleStart()
	{
        Debug.Log("Now invincible");
	}
    public void InvincibleEnd()
	{
        Debug.Log("Not invincible");
	}
}
