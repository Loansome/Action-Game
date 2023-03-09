using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EngineVariablesObject : ScriptableObject
{
    public List<string> rawInputBindings = new List<string>();

    public int holdButton = 5;
    public List<MovelistInput> baseMoveList;

}

[System.Serializable]
public class MovelistInput
{
    public int pressInput = -1;
    public State state;
}
