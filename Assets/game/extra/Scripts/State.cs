using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class State : ScriptableObject
{
    public string animationState;
    public bool hasAirVariantAnimation;   //When true Will append +"Air" or +"Aerial" to state name
                                          //when updating animator as to use same state data with animation variant
    public float animationBlend;
    //public float clipLength;
    public int length;
    public List<StateEvent> events = new List<StateEvent>();
    public List<StateTransition> transitions = new List<StateTransition>();
    public bool typicalStart = true;
    public List<Attack> attacks = new List<Attack>();

    public enum StateType { BASE, CHAIN, TRICK, CRUSH, SPECIAL, SWITCH }
    public StateType stateType;
    //public WeaponClass weaponClass;
    public bool aim;

    public float tpCost;

    public float dps = 1f;

    public bool breakCancel;
    public bool hitstunCancel;
    public bool bypassHitstunGuard;

    public bool cantLoop;
    public bool useJumps;

    public bool groundCancel;
}

[System.Serializable]
public class StateTransition
{
    public bool active = true;
    public CommandInput pressButton = new CommandInput();
    public CommandInput holdButton = new CommandInput();
    public State state;
    public bool ignoreHitstun;
    public bool ignoreCancel;
    public float earlyCancelWindow;
    public bool useJumps;
    public bool cantLoop;
    public enum AirState { Any, Ground, Air }
    public AirState airState;
    public enum TransitionPriority { DEFAULT_BASIC, CURRENT_BASIC, DEFAULT_SPECIAL, CURRENT_SEPCIAL }
    public int priority;
}



