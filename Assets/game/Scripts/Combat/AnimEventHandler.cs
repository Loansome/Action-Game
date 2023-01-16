using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;

public class AnimEventHandler : MonoBehaviour
{
    public MyCharacterController playerController;
    private float _gravityDefault;

    // Start is called before the first frame update
    void Start()
    {
        _gravityDefault = playerController.Gravity.y;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Cancelable()
	{

	}
    public void DamageStart()
	{
        Debug.Log("Can damage");
	}
    public void DamageEnd()
	{
        Debug.Log("End damage");
    }
    public void GravitySet(int newGravity)
	{
        playerController.Gravity.y = newGravity;
	}
    public void GravityReset()
	{
        playerController.Gravity.y = _gravityDefault;
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
