using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    //!! ADVISORY Turn off Baking on any custom hitbox meshes (mesh collider)
    public CharacterControl myCharacterControl;
    //public float lastHitActive;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void NewAttack(Attack _atk)
    {
        //if (myCharacterControl.controlType == CharacterControl.ControlType.BULLET) { return; }
        transform.localPosition = _atk.hitBoxPos;
        transform.localScale = _atk.hitBoxScale;
        transform.localEulerAngles = _atk.hitBoxRot;
    }

    //void LateUpdate()
    //{
    //    if (myCharacterControl.hitActive > 0)
    //    {
    //        NewAttack(myCharacterControl.currentAttack);
    //    }
    //}

    void OnTriggerStay(Collider other)
    {
        if (myCharacterControl.hitActive > 0)
        {
            //NewAttack(myCharacterControl.currentAttack); //To-DO Make this effecient!!
            //if (currentAttack != myCharacterControl.currentAttack) { NewAttack(myCharacterControl.currentAttack); }
            CharacterControl otherChar = other.GetComponent<CharacterControl>();
            if (otherChar != null)
            {
                if (otherChar != myCharacterControl)
                {
                    //Debug.Log("ATTACK HIT!!!!");
                    otherChar.GetHit(myCharacterControl);
                }
            }
        }
    }
}
