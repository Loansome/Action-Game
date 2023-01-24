using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;

public class AnimEventHandler : MonoBehaviour
{
    [SerializeField] private MyCharacterController playerController;
    private Vector3 _gravityPrevious;
    private Vector3 _gravityDefault;

    // Start is called before the first frame update
    void Start()
    {
        _gravityDefault = playerController.Gravity;
        _gravityPrevious = playerController.Gravity;
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
        _gravityPrevious = playerController.Gravity;
        playerController.Gravity *= gravityModifier;
	}
    public void GravityReset()
	{
        playerController.Gravity = _gravityPrevious; //_gravityDefault;
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
