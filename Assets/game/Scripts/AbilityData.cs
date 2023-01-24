using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Ability", menuName = "Ability")]
public class AbilityData : ScriptableObject
{
    public new string name;
    public string description;
    public int abilityPointCost;

	private void OnValidate()
	{
		name = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(this));
	}
}
