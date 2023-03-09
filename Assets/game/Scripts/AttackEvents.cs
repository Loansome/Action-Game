using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackEvents
{
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
        Debug.Log("New gravity");
    }
    public void GravityReset()
    {
        //playerController.Gravity = _gravityPrevious; //_gravityDefault;
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
