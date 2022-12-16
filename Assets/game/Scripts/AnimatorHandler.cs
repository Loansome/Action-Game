using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHandler : MonoBehaviour
{
    [SerializeField] GameObject player;
    CharacterController charaMove;
    PlayerAttack pAttack;

    private void Awake()
    {
        pAttack = player.GetComponent<PlayerAttack>();
        charaMove = player.GetComponent<CharacterController>();
    }

    private void OnAnimatorMove()
    {
        //if (pAttack.currentComboState == ComboState.NONE) return;
        //else
        {
            charaMove.Move(GetComponent<Animator>().deltaPosition);
        }
    }
}
