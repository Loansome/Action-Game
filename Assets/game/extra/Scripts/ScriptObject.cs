using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ScriptObject : ScriptableObject
{
    public List<ScriptParameter> parameters = new List<ScriptParameter>();
}

[System.Serializable]
public class ScriptParameter
{
    public float value;
    public string name;
}

[System.Serializable]
public class StateEvent
{
    public bool active = true;
    public float start;
    public float end;

    public string script = "";
    public CharacterControl.ScriptOperation operation;
    public List<float> parameters = new List<float>();
    public List<object> args = new List<object>();
    public GameObject prefab;
    //public List<variable> vars = new List<object>();

    public enum AirCondition { OK, AIR, GRND }
    public AirCondition airCondition;
    public bool holdButton;

    //Helpers for the editor
    public float scriptIndex;
    public string label;



}
