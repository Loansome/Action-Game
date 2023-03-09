using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Ability", menuName = "New Ability")]
public class AbilityData : ScriptableObject
{
	[Header("Ability Info")]
    public new string name;
    public string description;
    public int abilityPointCost;

	private void OnValidate()
	{
		name = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(this));
	}
}
