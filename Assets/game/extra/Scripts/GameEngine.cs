using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameEngine : MonoBehaviour
{
    public static GameEngine current;

    public Vector3 friction = new Vector3(0.875f, 0.975f, 0.875f);
    public Vector3 lightHitstunFriction = new Vector3(0.925f, 0.925f, 0.925f);
    public Vector3 heavyHitStunFriction = new Vector3(0.99f, 0.99f, 0.99f);
    public float pushbackFriction = 0.95f;
    public Vector3 gravity = new Vector3(0f, -0.013f, 0f);
    public bool pauseUpdate;

    public int enemyCount;

    public int globalHitpause;
    public int counterhitWindow = 5;
    public int comboTimerMax = 300;

    public HitEffectManager hitManager;

    public EngineVariablesObject variablesObject;
    public SystemFile systemFile;
    public int currentSaveFile;
    public bool drawDebug = true;

    public DynamicCameraControl mainCameraControl;
    
    public float currentZ;


    public CharacterControl mainCharacter;

   
    public State aiTestState;

   

    public float deadZone = 0.01f;

    public int breakStateWindow = 15; //Length in frames player can't cancel out of break state
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        current = this;
    }

    // Update is called once per frame
    void Update()
    {
        enemyCount = 0;
        if (globalHitpause > 0) { globalHitpause--; }
        
    }

    public SaveFile GetCurrentSaveFile()
    {
        return systemFile.saveFiles[currentSaveFile];
    }
    void FixedUpdate()
    {
       
    }

    public void DispatchAttack()
    {
        List<CharacterControl> enemyList = new List<CharacterControl>();
        
        foreach (CharacterControl chara in GameObject.FindObjectsOfType<CharacterControl>())
        {
            if (chara != mainCharacter) 
            {
                if (chara.controlType == CharacterControl.ControlType.AI) 
                {
                    enemyList.Add(chara);
                }
            }
        }
        if (enemyList.Count > 0)
        {
            enemyList[Random.Range(0, enemyList.Count)].StartBasicAIAttack();
        }
    }

    public bool InHitpause()
    {
        if (globalHitpause > 0) { return true; }
        return false;
    }

    public void SetHitpause(int _hitpause)
    {
        if (_hitpause > globalHitpause) { globalHitpause = _hitpause; }
    }

    public void RegisterEnemyCount()
    {
        enemyCount++;
    }

    public void DeactivateCharactersForSceneLoad()
    {
        foreach (CharacterControl chara in GameObject.FindObjectsOfType<CharacterControl>())
        {
            if (chara != mainCharacter) { Destroy(chara); }
        }
    }

    public void ScreenShake(float _pow, float _damp, int _time)
    {
        //mainCameraControl.NewShake(_pow, _damp, _time);
    }

    void OnGUI()
    {
        if (GameEngine.current.drawDebug)
        {
            Rect currentRect = new Rect(200, 1, 100, 100);

        }
    }

}
