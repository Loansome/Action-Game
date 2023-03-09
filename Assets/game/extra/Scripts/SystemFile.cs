using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SystemFile : ScriptableObject
{
    public List<SaveFile> saveFiles = new List<SaveFile>();
}

[System.Serializable]
public class SaveFile
{
    public Player player;
}

[System.Serializable]
public class Player
{
    public string userName = "<NEW PLAYER>";
    public float currentExperience;
    public int currentLevel;

    public int currentWeapon = 0;

    

    public void SwitchCurrentWeapon()
    {
        if (currentWeapon == 0) { currentWeapon = 1; }
        else { currentWeapon = 0; }
    }
}


