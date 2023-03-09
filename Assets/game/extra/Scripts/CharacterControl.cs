using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    //public List<WeaponSlot> weapons = new List<WeaponSlot>();
    public float moveSpeed;
    public float stickX;
    public float stickY;
    public Vector3 velocity;
    public Vector3 pushBack;

    public CharacterController myCharacterController;
    public int team;
    public enum ControlType { CONTROLLER, BULLET, AI }

    public ControlType controlType;

    public int facingDir = 1;
    public bool faceVelocity;

    public GameObject bulletPrefab;
    public int bulletTime;
    public int bulletTimeMax = 60;

    public CharacterControl attackTarget;
    public Hitbox hitBox;
    public float health;
    public float healthMax = 9;

    public float armorCurrent;
    public float armorMax;
    public float armorRatio;   //Percent based on maxHealth;
    public float armorReduction;  //Damage reduction on armor
    public float armorRegen;      //Rate at which armor regenerates while active
    public float armorRestore;    //Rate at which armor returns after being broken: use this sparingly--in aces wild kayin smartly had us remove it from all bosses

    public int guardPower;

    public float guardReduction;
    public float hitstunGuardRate = 0.01f;
    public float hitstunGuardChance;

    public float guardCancelRate = 0.1f;
    public float guardCancelChance;

    public float TP;
    public float TPMAX = 100;
    public float TPRegen = 0.05f;

    public float MP;
    public float MPMAX = 5;
    public float ammoCurrent = 5;
    public float ammoMAX = 5;
    public float reloadCurrent;
    public float reloadMAX;


    public float drive;
    public float driveMAX;

    public float overdrive;
    public float driveBreak;

    public float stunCurrent;
    public float stunMax;
    public float stunTime;

    public int currentCombo;
    public int currentComboTimer;


    public bool piercing;
    public int currentJobTime;
    public int jobTimeMax = 60;

    public float ATB;
    public float ATBMax = 999;
    public float ATBSpeed = 4;

    public Transform bulletFirePoint;
    public CharacterControl projectileParent;
    public GameObject spawnVFX;
    public GameObject despawnVFX;
    public GameObject hitVFX;

    //public bool hit;
    public int hitActive;
    public int hitConfirm;
    public bool hitConfirmed;

    public bool aerial;
    public int aerialTimer;
    public bool ignorePhysics;
    public int ignorePhysicsTimer;
    public bool ignoreGravity;
    public bool flying;
    public bool ignoreFriction;

    public int jumps;
    public int jumpsMax = 2;

    public int dodges;
    public int invincibility;

    public bool canCancel;

    public int hitStun;
    public Attack.HitPower hitPower;
    public Attack.HitDirection hitDir;
    public float hitDirPow;
    //public DrawEffects drawEffects;

    public Vector3 respawnPoint;

    public float fireMoveMod = 0.9f;

    public int bulletRepeatCount;

    public bool mirrorX;

    [HideInInspector]
    public List<CharacterControl> hitList = new List<CharacterControl>();

    public State defaultState;
    public State currentState;
    public State prevState;
    public int currentStateFrame;
    public int totalFramesInState;
    public Animator myAnimator;

    public Attack currentAttack;
    public int currentChain;

    public float runAniSpeed;
    
    public GameObject myCharacterTransform;
    public Vector3 characterTransformStartPosition;
    public Vector2 currentShake;
    public float shakePow;
    public float shakeDamp;
    public int shakeTime;

    public float experience = 10f;

    public SegmentedSimulation capeControl;

    // Start is called before the first frame update
    void Start()
    {
        respawnPoint = transform.position;
        Initialize();
        if (controlType == ControlType.AI) { moveSpeed += Random.Range(-0.001f, 0.001f); }
        characterTransformStartPosition = myCharacterTransform.transform.localPosition;
    }
    public InputBuffer inputBuffer;


    public void Initialize()
    {
        health = healthMax;
        TP = TPMAX;

        if (controlType == ControlType.CONTROLLER) { inputBuffer = new InputBuffer(); inputBuffer.Initialize(); }
        if (controlType != ControlType.BULLET) { StartState(currentState); }
        if (controlType == ControlType.AI) { currentStateFrame += Random.Range(0, 20); }
    }

    public void Respawn()
    {
        transform.position = respawnPoint;
        Initialize();
    }

    public bool PauseUpdate()
    {
        if (hitPause > 0 ||
            GameEngine.current.globalHitpause > 0 ||
            GameEngine.current.pauseUpdate ) { return true; }
        return false;
    }

    public void Guard(float _lv, float _red)
    {
        guardPower = (int)_lv;
        guardReduction = _red;
    }
    public float GetHealthPercent()
    {
        if (healthMax == 0) { return 0f; }
        return health / healthMax;
    }

    public float GetMagicPercent()
    {
        if (MPMAX == 0) { return 0f; }
        return MP / MPMAX;
    }

    public float GetTechPercent()
    {
        if (TPMAX == 0) { return 0f; }
        return TP / TPMAX;
    }

    public float GetOverdrivePercent()
    {
        if (driveMAX == 0) { return 0f; }
        return overdrive / driveMAX;
    }

    public float GetDrivePercent()
    {
        if (driveMAX == 0) { return 0f; }
        return drive / driveMAX;
    }

    public float GetDriveAndOverdrivePercent()
    {
        if (driveMAX == 0) { return 0f; }
        return (drive + overdrive) / driveMAX;
    }

    int updateTicks = 0;
    // Update is called once per frame
    void FixedUpdate()
    {
        updateTicks++;
        //Debug.Log("Fixed Update : " + updateTicks);
        //if (GameEngine.current.sceneTransitioner.inTransition) { return; }
        if (controlType == ControlType.AI && team == 1) { GameEngine.current.RegisterEnemyCount(); }
        if (controlType == ControlType.CONTROLLER) { UpdateInput(); }
        //if (GameEngine.current.pauseUpdate) { return; }
        UpdateShake();
        if (hitPause > 0) { hitPause--; }
        else if (GameEngine.current.globalHitpause > 0) { }
        else if (GameEngine.current.pauseUpdate) { }
        //else if (GameEngine.current.sceneTransitioner.inTransition) { }
        else
        {

            if (stunTime <= 0 && hitStun <= 0)
            {
                switch (controlType)
                {
                    //case ControlType.CONTROLLER: UpdateInput(); break;
                    case ControlType.AI: UpdateAI(); break;
                    case ControlType.BULLET: UpdateBullet(); break;
                }
            }

            UpdateTimers();

            UpdateState();
            UpdatePhysics();

            //UpdateAnimation();
        }

        UpdateAnimation();

    }

    public void UpdateShake()
    {
        if (shakeTime > 0)
        {
            shakeTime--;
            currentShake = new Vector2(Random.Range(-10f, 10), Random.Range(-10f, 10f));
            currentShake.Normalize();
            currentShake *= shakePow;
            shakePow *= shakeDamp;
            myCharacterTransform.transform.localPosition = characterTransformStartPosition + new Vector3(0, currentShake.y, currentShake.x);
        }
        else
        {
            myCharacterTransform.transform.localPosition = characterTransformStartPosition;
        }
    }

    void BasicShake()
    {
        shakeTime = 5 + GameEngine.current.globalHitpause; // GameEngine.current.globalHitpause;
        shakePow = 0.1f;
        shakeDamp = 0.89f;
    }

    public void UpdateCancel()
    {

    }

    public void FaceAttackTarget()
    {
        if (controlType == ControlType.AI) { attackTarget = GameEngine.current.mainCharacter; }
        FaceTargetCharacter(attackTarget);
    }

    State UpdateMovelist(List<MovelistInput> _moveList)
    {
        //if (Input.GetButtonDown("LB")) { GameEngine.current.GetCurrentSaveFile().player.SwitchCurrentWeapon(); TP = TPMAX; }
        State nextState = null;
        for (int i = 0; i < _moveList.Count; i++)
        {
            MovelistInput m = _moveList[i];


            if (inputBuffer.buttons[m.pressInput].pressOK > -1)
            {
                State curState = null;
                if (m.state != null) { curState = m.state; }
                else
                {
                    List<State> stateList = new List<State>();// nextWeaponClass.chainAttacks;
                    int nextIndex = currentChain;
                    //Special Attack
                    if (inputBuffer.buttons[GameEngine.current.variablesObject.holdButton].
                        buffer[inputBuffer.buttons[m.pressInput].pressOK].hold > 0)
                    {
                        //stateList = nextWeaponClass.specialAttacks;
                        nextIndex = m.pressInput;
                    }
                    else //Chain Attack / Trick / Crush
                    {
                        if (m.pressInput == 1)
                        {
                            
                        }
                        else
                        {

                        }
                    }
                    if (nextIndex > stateList.Count - 1) { nextIndex = 0; }
                    curState = stateList[nextIndex];
                }

                if (TP >= curState.tpCost)
                {
                    if (hitStun <= 0 || curState.breakCancel || curState.hitstunCancel)
                    {
                        if (canCancel || (curState.hitstunCancel && hitStun > 0)
                            || (curState.breakCancel &&
                                  //currentState == GameEngine.current.hitManager.genericGuardBreak &&
                                  totalFramesInState >= GameEngine.current.breakStateWindow))
                        {
                            if (!curState.useJumps || jumps > 0)//jumps >= t.useJumps)
                            {
                                if (!curState.cantLoop || currentState != curState)
                                {
                                    if (curState.useJumps) { jumps -= 1; }
                                    inputBuffer.lastButtonUsed = m.pressInput;
                                    inputBuffer.buttons[m.pressInput].buffer[inputBuffer.buttons[m.pressInput].pressOK].used = true;


                                    //Debug.Log("Transition to => " + t.state.name);
                                    //currentPriority = t.priority;
                                    nextState = curState;
                                    //return t.state;
                                }
                            }
                        }
                    }
                }
            }


        }
        return nextState;
    }

    State UpdateTransitions(List<StateTransition> _list)
    {
        State nextState = null;
        for (int i = 0; i < _list.Count; i++)
        {
            StateTransition t = _list[i];
            if (t.active)
            {
                if (t.priority > currentPriority)
                {
                    if (t.pressButton.active && inputBuffer.buttons[t.pressButton.buttonIndex].pressOK > -1)
                    {
                        if (!t.holdButton.active ||
                            (t.holdButton.active &&
                            inputBuffer.buttons[t.holdButton.buttonIndex].buffer[inputBuffer.buttons[t.pressButton.buttonIndex].pressOK].hold > 0))
                        {
                            if (TP >= t.state.tpCost)
                            {
                                if (hitStun <= 0 || t.state.breakCancel || t.state.hitstunCancel)
                                {
                                    if (canCancel || t.ignoreCancel || (t.state.hitstunCancel && hitStun > 0)
                                        || (t.state.breakCancel &&
                                              //currentState == GameEngine.current.hitManager.genericGuardBreak &&
                                              totalFramesInState >= GameEngine.current.breakStateWindow))
                                    {
                                        if (!t.useJumps || jumps > 0)//jumps >= t.useJumps)
                                        {
                                            if (t.airState == StateTransition.AirState.Any ||
                                                (t.airState == StateTransition.AirState.Ground && !aerial) ||
                                                (t.airState == StateTransition.AirState.Air && aerial))
                                            {
                                                if (!t.cantLoop || currentState != t.state)
                                                {
                                                    if (t.useJumps) { jumps -= 1; }
                                                    inputBuffer.lastButtonUsed = t.pressButton.buttonIndex;
                                                    inputBuffer.buttons[t.pressButton.buttonIndex].buffer[inputBuffer.buttons[t.pressButton.buttonIndex].pressOK].used = true;


                                                    //Debug.Log("Transition to => " + t.state.name);
                                                    currentPriority = t.priority;
                                                    nextState = t.state;
                                                    
                                                    //return t.state;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        
        
        return nextState;
    }

    public void BreakDrive()
    {
        driveBreak = overdrive;
        ChangeDrive(0f, -overdrive);
    }

    public float lastAniTime = 0;
    public void StartState(State _nextState)
    {

        if (_nextState == null) { return; }
        Debug.Log("Start State => " + _nextState.name);
        if (currentState.length == 0) { lastAniTime = 0; }
        else { lastAniTime = currentStateFrame / currentState.length; }
        prevState = currentState;
        currentState = _nextState;

        currentStateFrame = 0;
        totalFramesInState = 0;
        ignoreGravity = false;
        canCancel = false;
        hitConfirm = 0;
        dodges = 0;
        invincibility = 0;
        driveBreak = 0;
        guardPower = 0;
        hitActive = 0;
        hitStun = 0;
        hitConfirmed = false;
        curStateLoops = 0;
       
        foreach (VFXControl vfxCon in GetComponentsInChildren<VFXControl>())
        {
            vfxCon.Deparent();
        }

        //transform.rotation = Quaternion.LookRotation(new Vector3(transform.localScale.x, 0, 0), Vector3.up);
        if (currentState.typicalStart && controlType == ControlType.CONTROLLER) { FaceStick(1f); }
        if (currentState.stateType == State.StateType.CHAIN || currentState.stateType == State.StateType.TRICK)
        {
            currentChain++;
            
            if (currentState.stateType == State.StateType.TRICK)
            {
                currentChain = 0;
            }
            if (currentState.stateType == State.StateType.SWITCH)
            {
                GameEngine.current.GetCurrentSaveFile().player.SwitchCurrentWeapon();
                currentChain = 0;
            }
        }
        else { currentChain = 0; }
        if (currentState == defaultState) { canCancel = true; }
        //if (currentState.aim) { myAnimator.SetFloat("aimDir", stickY); }
    }

    int currentPriority;
    void UpdateState()
    {
        if (currentState != null)
        {
            //if (hitStun <= 0) { currentStateFrame++; }
            currentStateFrame++;
            totalFramesInState++;
            if (currentStateFrame > currentState.length) 
            {
                if (controlType == ControlType.BULLET && bulletTimeMax < 0) { Despawn(); }
                else { StartState(defaultState); }
            }


            if (controlType == ControlType.CONTROLLER)
            {
                currentPriority = -1;
                List<StateTransition> nextTransitions = new List<StateTransition>();
                nextTransitions.AddRange(currentState.transitions);
                if (currentState != defaultState) { nextTransitions.AddRange(defaultState.transitions); }
                //if (currentState != defaultState && nextState == null) { nextState = CheckTransitions(defaultState.transitions); }

                //State nextState = UpdateMovelist(GameEngine.current.variablesObject.baseMoveList);
                
                State nextState = UpdateTransitions(nextTransitions);
                if (currentState.groundCancel && !aerial) { nextState = defaultState; }
                if (nextState != null) { prevAniState = ""; StartState(nextState); }
                if (currentState == defaultState) { DefaultRotation(1f); }

            }

            

            for (int i = 0; i < currentState.events.Count; i++)
            {
                StateEvent ev = currentState.events[i];
                if (ev.active)
                {
                    if (CheckAirCondition(ev.airCondition))
                    {
                        if (CheckHoldCondition(ev.holdButton))
                        {
                            ExecuteEvent(ev, currentStateFrame);
                        }
                    }
                }
            }

            for (int a = 0; a < currentState.attacks.Count; a++)
            {
                Attack atk = currentState.attacks[a];
                //int curFrame = (int)(currentStateFrame * currentState.length);

                if (atk.active)
                {
                    int cancelStart = (int)(atk.cancelWindowStart * currentState.length);
                    //int cancelLength = (int)(atk.cancelWindowStart * currentState.length);
                    if ((hitConfirmed && currentStateFrame >= cancelStart) ||
                         currentStateFrame >= cancelStart + Attack.whiffCancelWindow)
                    {
                        CanCancel(1f);
                    }

                    
                    if (currentStateFrame == (int)(atk.startFrame * currentState.length))
                    {
                        hitActive = (int)(atk.endFrame * currentState.length) - (int)(atk.startFrame * currentState.length);
                        hitList = new List<CharacterControl>();
                        currentAttack = atk;
                        currentAttackIndex = a;
                        hitBox.NewAttack(currentAttack);
                        hitConfirm = 0;
                    }
                }
            }

        }

    }

    public bool CheckHoldCondition(bool _hold)
    {
        if (_hold && !inputBuffer.HoldingLastCancelButton()) { return false; }
        return true;
    }

    public bool CheckAirCondition(StateEvent.AirCondition _cond)
    {
        switch (_cond)
        {
            case StateEvent.AirCondition.OK: return true;
            case StateEvent.AirCondition.AIR: if (aerial) { return true; } break;
            case StateEvent.AirCondition.GRND: if (!aerial) { return true; } break;
        }
        return false;
    }

    public string prevAniState;
    public string currentAniState;
    public void UpdateAnimation()
    {
        if (myAnimator == null) { return; }

        

            currentAniState = currentState.animationState;
        if (currentState.hasAirVariantAnimation && aerial && !flying) { currentAniState = "Air" + currentAniState; }

        //int nextHash = Animator.StringToHash(nextAniState);
        //int currentHash = myAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash;

        //Debug.Log(currentHash);


        if (prevAniState != currentAniState)//!myAnimator.GetCurrentAnimatorStateInfo(0).IsName(currentAniState))
        {
            float nextBlend = currentState.animationBlend;
            if (currentState == defaultState && prevState.attacks.Count > 0)
            {
               
                    nextBlend = currentState.animationBlend + 0.2f; //Soften animation blend when going from attacks to neutral
                if (prevState.groundCancel && !aerial)
                {
                    nextBlend = currentState.animationBlend;
                }
            }
            //Debug.Log("CROSSFADE ME!!!");
            //myAnimator.tim
            //myAnimator.CrossFade(currentAniState, currentState.animationBlend);
            //myAnimator.CrossFade(currentAniState, 0);
            if (controlType != ControlType.BULLET)//(currentAniState != "")
            {
                //myAnimator.Play(currentAniState, 0);
                //myAnimator.CrossFadeInFixedTime(currentAniState,currentState.animationBlend,0,)
                Debug.Log("PREV ANI: --> " + prevAniState);
                if (myAnimator.GetCurrentAnimatorStateInfo(0).IsName(currentAniState)) { myAnimator.Play(currentAniState, 0, 0); }
                else { myAnimator.CrossFade(currentAniState, nextBlend); }
                
                
                prevAniState = currentAniState;
                Debug.Log("CROSSFADE: --> " + currentAniState);
            }
        }
        if (currentState.length > 0)
        {
            //myAnimator.ForceStateNormalizedTime((float)currentStateFrame / (float)currentState.length);
            //myAnimator.GetCurrentAnimatorStateInfo(0).
            //myAnimator.SetFloat("normalizedTime", (float)currentStateFrame / (float)currentState.length);
            //myAnimator.SetFloat(currentAniState + "_" + "normalizedTime", (float)currentStateFrame / (float)currentState.length);
        }
        if (hitStun > 0)
        {
            myAnimator.SetFloat("hitDir", hitDirPow);
            float targetDir = 0.5f;
            float targetRate = 0.2f;
            if (hitDir == Attack.HitDirection.HIGH) { targetDir = 1f; }
            else { targetDir = 0f; }
            if (hitStun < 12) { targetDir = 0.5f; targetRate = 0.1f; }

            hitDirPow += (targetDir - hitDirPow) * targetRate;

        }
        float aniSpeed = 1f;
        aniSpeed = 60f / (float)currentState.length * 2.4f;
        if (hitPause > 0) { aniSpeed *= 0f; }
        if (GameEngine.current.globalHitpause > 0) { aniSpeed *= 0f; }
        if (GameEngine.current.pauseUpdate) { aniSpeed *= 0f; }
        //if (GameEngine.current.sceneTransitioner.inTransition) { aniSpeed *= 0f; }
        myAnimator.SetFloat("moveSpeed", runAniSpeed);// Mathf.Abs(velocity.x));
        myAnimator.SetFloat("yDir", yDir);
        myAnimator.SetFloat("speed", aniSpeed);
        myAnimator.SetFloat("aimDir", curAimDir);
    }


    public void HairForce(float _pow, float _decay, float _dir)
    {
        Vector3 nextForce = _pow * transform.forward;
        nextForce = Quaternion.AngleAxis(_dir * transform.forward.x, Vector3.forward) * nextForce;
        //To-Do: Rotate nextForce by _dir
        capeControl.ApplyForce(nextForce, _decay);
    }



    public float curAimDir;
    public float aimSpeed = 0.25f;
    void UpdateTimers()
    {

        float targetY = Input.GetAxisRaw("Vertical");
        float targetX = Input.GetAxisRaw("Horizontal");
        if ((transform.forward.x < 0 && targetX > 0) || (transform.forward.x > 0 && targetX < 0))
        {
            if (targetY > 0) { targetY = 1; }
            else { targetY = -1; }
        }
        curAimDir += (targetY - curAimDir) * aimSpeed;
        runAniSpeed *= 0.99f;
        //runMomentum *= 0.5f;
        if (hitActive > 0) { hitActive--; }
        if (stunTime > 0) { stunTime--; }
        if (hitStun > 0) { hitStun--; }
        if (invincibility > 0) { invincibility--; }
        if (currentComboTimer > 0) { currentComboTimer--; }
        else { currentCombo = 0; }
        ChangeTP(TPRegen);
        UpdateDrive();
    }

    float driveDecay = 0.1f;
    void UpdateDrive()
    {
        if (overdrive > 0) { overdrive -= driveDecay; }
        ClampDrive();
    }

    void ClampDrive()
    {
        if (overdrive + drive > driveMAX)
        {
            drive -= (overdrive + drive) - driveMAX;
        }

        if (overdrive < 0) { overdrive = 0; }
        if (overdrive > driveMAX) { overdrive = driveMAX; }

        if (drive < 0) { drive = 0; }
        if (drive > driveMAX) { drive = driveMAX; }

        //if (overdrive > drive) { drive = overdrive; }

    }

    void ChangeDrive(float _dVal, float _odVal)
    {
        drive += _dVal;
        overdrive += _odVal;
        ClampDrive();
    }

    void Bide()
    {
        if (ATB < ATBMax) { ATB += ATBSpeed; }
        if (ATB > ATBMax) { ATB = ATBMax; }
    }



    //public JobSetItem currentAIAttack;

    void Approach()
    {
        if (attackTarget == null) { return; }
        targetOffset = (attackTarget.transform.position - transform.position).normalized;
        float targetDistance = Vector3.Distance(attackTarget.transform.position, transform.position);
        float debugDistance = 5;

        if (targetDistance > debugDistance) { AIMove(); }
    }
    void AIMove()
    {

        if (flying) { velocity += targetOffset * moveSpeed * 0.35f; }
        else { }

        runAniSpeed += (Mathf.Abs(velocity.x * 1.1f) - runAniSpeed) * 0.5f;
    }

    void AttackFromRange()
    {
        //Basic AI pattern where you pick an attack based on your range from the player
        //rather than moving into range then attacking
        //Would be good to pick between this and the Approach>Attack style randomly
    }

    void JobLoop()
    {
        currentJobTime--;
        if (currentJobTime < 0) { currentJobTime = 0; }
    }

    void UpdateAttackTarget()
    {
        attackTarget = GameEngine.current.mainCharacter;
    }
    void UpdateAI()
    {
        UpdateAttackTarget();
        Approach();
        //FaceTarget(0.07f);
        //FrontVelocity(moveSpeed, ScriptEvent.Operation.ADD);
        Bide();

        currentJobTime++;
        if (currentJobTime >= jobTimeMax)
        {
            currentJobTime = 0 + Random.Range(0, 30);
            //FireBullet(bulletPrefab, null, 0); 
            StartState(GameEngine.current.aiTestState);
        }
        FaceAttackTarget();
    }

    Vector3 targetOffset;
    public void FaceTarget(float _pow)
    {
        if (attackTarget == null) { return; }
        targetOffset = attackTarget.transform.position - transform.position;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(targetOffset.x, 0, targetOffset.z), Vector3.up), _pow);
    }

    void FaceTargetCharacter(CharacterControl _target)
    {
        if (_target == null) { return; }
        targetOffset = _target.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(new Vector3(targetOffset.x, 0, targetOffset.z), Vector3.up);
    }

    public void ChangeTP(float _val)
    {
        TP += _val;
        TP = Mathf.Clamp(TP, 0, TPMAX);
    }

    public int fireInterval = 10;
    public int fireCooldown = 0;

    public int autoFireMax = 15;
    public int autoFireCurrent;
    public void UpdateInput()
    {
        //CameraRelativeMove();
        //EasyJump();
        stickX = Input.GetAxisRaw("LeftStickX");
        stickY = Input.GetAxisRaw("LeftStickY");
        //Debug.Log("LT : " + Input.GetAxisRaw("LT"));
        //Debug.Log("RT : " + Input.GetAxisRaw("RT"));
        inputBuffer.Update();
    }

    public float jumpVelocity = 0.35f;
    public void EasyJump()
    {
        if (Input.GetButtonDown("Fire1")) { velocity.y = jumpVelocity; }
    }

    Vector3 velDir;
    Vector3 velHelp;


    public float runMomentum = 0.0125f;
    public int runTimer;
    public int runTimerMax = 120;

    public void CameraRelativeMove(float _pow)
    {
        if (_pow == 0) { _pow = moveSpeed; }

        velHelp = new Vector3(0, 0, 0);
        velDir = -Camera.main.transform.forward;
        velDir.y = 0;
        velDir.Normalize();
        velHelp += velDir * stickY;

        velHelp += Camera.main.transform.right * stickX;
        velHelp.y = 0;
        velHelp.Normalize();

        if (Mathf.Abs(stickX) > GameEngine.current.deadZone || Mathf.Abs(stickY) > GameEngine.current.deadZone) 
        {
            if (runTimer < runTimerMax)
            {
                if (!aerial)
                {
                    runTimer += 1;
                }
            }
            //if (runTimer >= runTimerMax) { _pow += runMomentum; }
            velocity += velHelp * (_pow);

        }
        else { runTimer = 0; }
        runAniSpeed += (Mathf.Abs(Vector2.SqrMagnitude(new Vector2(velocity.x,velocity.z)) * 1.1f) - runAniSpeed) * 0.5f;
        
    }

    public void SetFacingDir(float _compare)
    {
        if (_compare > 0) { facingDir = 1; }
        if (_compare < 0) { facingDir = -1; }
    }
    public void IgnoreGravity(float _opr)
    {
        switch (_opr)
        {
            case 0: ignoreGravity = false; break;
            case 1: ignoreGravity = true; break;
        }
    }

    public void CanCancel(float _opr)
    {
        switch (_opr)
        {
            case 0: canCancel = false; break;
            case 1: canCancel = true; break;
        }
    }

    public void IntMethod(int _pow)
    {
        velocity.x = _pow;
    }

    public void FrontAndStickVelocity(float _pow, float _opr, float _stk)
    {
        FrontVelocity(_pow, _opr);
        CameraRelativeMove(_stk);
    }

    public void FaceStickBasic()
    {
        if (Mathf.Abs(stickX) > GameEngine.current.deadZone)
        {
            SetFacingDir(stickX);
        }
        transform.rotation = Quaternion.LookRotation(new Vector3(facingDir, 0, 0), Vector3.up);

    }
    public void FaceStick(float _pow)
    {
        if (CheckStickDeadzone())
        {
            velHelp = new Vector3(0, 0, 0);
            velDir = -Camera.main.transform.forward;
            velDir.y = 0;
            velDir.Normalize();
            velHelp += velDir * stickY;

            velHelp += Camera.main.transform.right * stickX;
            velHelp.y = 0;

            myCharacterTransform.transform.rotation =
                Quaternion.Lerp(myCharacterTransform.transform.rotation,
                Quaternion.LookRotation(new Vector3(velHelp.x, 0, velHelp.z), Vector3.up), _pow);
        }

    }

    public float simpleRotation;
    public float fireRotation;

    public float yDir;
    public void UpdatePhysics()
    {
        velocity.Scale(GameEngine.current.friction);
        velocity += GameEngine.current.gravity;
        myCharacterController.Move(velocity + pushBack);

        if ((myCharacterController.collisionFlags & CollisionFlags.Below) != 0)
        {
            if (velocity.y < 0) { velocity.y *= 0.25f; }
            if (aerialTimer > 0) { yDir *= 3; }
            aerialTimer = 0;
            aerial = false;
            jumps = jumpsMax;
            yDir *= 0.8f; // 0.7f;
        }
        else
        {
            float yDirTarget = Mathf.InverseLerp(-1f, 1f, -4f * velocity.y);
            yDir += (yDirTarget - yDir) * 0.5f;
            if (aerialTimer < 60)
            {
                aerialTimer++;
                if (aerialTimer > 3) { aerial = true;  yDir = yDirTarget; }
                if (aerialTimer > 10) { if (jumps > 1) { jumps = 1; } }
            }
        }

        return;

        if (flying && hitStun <= 0) { ignoreGravity = true; }
        if (ignorePhysicsTimer > 0)
        {
            ignorePhysicsTimer--;
            return;
        }
        if (ignorePhysics) { return; }
        if (faceVelocity)
        {
            simpleRotation = Vector2.Angle(new Vector2(velocity.x, velocity.y), Vector2.up);
            transform.rotation = Quaternion.LookRotation(velocity, Vector3.forward);
        }
        else { simpleRotation = 0; }

        if (!ignoreGravity)
        {
            velocity += GameEngine.current.gravity;
        }



        pushBack *= GameEngine.current.pushbackFriction;
        myCharacterController.Move(velocity + pushBack);


        if ((myCharacterController.collisionFlags & CollisionFlags.Below) != 0)
        {
            if (velocity.y < 0) { velocity.y *= 0.25f; }
            if (aerialTimer > 0) { yDir *= 5; }
            aerialTimer = 0;
            aerial = false;
            jumps = jumpsMax;
            yDir *= 0.7f;
        }
        else
        {
            float yDirTarget = Mathf.InverseLerp(-1f, 1f, -4f * velocity.y);
            yDir += (yDirTarget - yDir) * 0.1f;
            if (aerialTimer < 60)
            {
                aerialTimer++;
                if (aerialTimer > 3) { aerial = true; yDir = yDirTarget; }
                if (aerialTimer > 10) { if (jumps > 1) { jumps = 1; } }
            }
        }


        if (!ignoreFriction)
        {
            if (hitStun > 0)
            {
                if (hitPower == Attack.HitPower.LIGHT) { velocity.Scale(GameEngine.current.lightHitstunFriction); }
                else { velocity.Scale(GameEngine.current.heavyHitStunFriction); }
            }
            else
            {
                if (ignoreGravity)
                {
                    velocity.Scale(new Vector3(
                                   GameEngine.current.friction.x, GameEngine.current.friction.x, GameEngine.current.friction.x));
                }
                else { velocity.Scale(GameEngine.current.friction); }
            }
        }
        //UpdateFacingDraw();
        //transform.position = new Vector3(transform.position.x, transform.position.y, GameEngine.current.currentZ);
    }

    public void HitstunLoop()
    {
        if (hitStun > 0) { currentStateFrame--; }
        else { StartState(defaultState); }
    }
    

    bool CheckStickDeadzone()
    {
        if (Mathf.Abs(stickX) < GameEngine.current.deadZone && Mathf.Abs(stickY) < GameEngine.current.deadZone) { return false; }
        return true;
    }
    bool CheckVelocityDeadzone()
    {
        if (Mathf.Abs(velocity.x) > GameEngine.current.deadZone * 0.01f) { return true; }
        //if (velocityZ > 0.001f) { return true; }
        //if (velocityZ < -0.001f) { return true; }
        return false;
    }

    public void FaceVelocity(float _fly)
    {
        if (CheckVelocityDeadzone())
        {
            if (_fly > 0) { transform.rotation = Quaternion.LookRotation(velocity, Vector3.up); }
            else { myCharacterTransform.transform.rotation = Quaternion.LookRotation(new Vector3(velocity.x, 0, velocity.z), Vector3.up); }
            
        }
    }

    public void FireBulletDefault()
    {
        FireBullet(bulletPrefab);
    }
    public float fireSinRate = 8f;
    public float fireSinePow = 0.25f;

    public void FireProjectile(GameObject _bulletPrefab)//, Weapon _weaponObject, float _simpleRotation)
    {
        GameObject nextObject = Instantiate(_bulletPrefab, transform.position, transform.rotation);

        //nextObject.transform.Rotate(new Vector3(0, -90 * facingDir, 0));
        CharacterControl nextCharacter = nextObject.GetComponent<CharacterControl>();
        nextCharacter.projectileParent = this;
        nextCharacter.facingDir = facingDir;
        nextCharacter.StartState(nextCharacter.currentState);
        
        nextCharacter.team = team;
        nextCharacter.velocity = transform.forward * 0.5f;
        nextCharacter.velocity.z = 0;
        
        //nextCharacter.StartState(nextCharacter.defaultState);
        //nextCharacter.FixedUpdate();

    }

    public void FireBullet(GameObject _bulletPrefab)//, Weapon _weaponObject, float _simpleRotation)
    {

        fireCooldown = fireInterval;
        GameObject nextObject = Instantiate(_bulletPrefab, bulletFirePoint.position, transform.rotation);//Quaternion.Euler(90, 0, 0));
        //nextObject.transform.position += new Vector3(0, 0, 0.2f);
        //nextObject.transform.Rotate(Vector3.up, _simpleRotation);

        if (bulletFirePoint != null) { nextObject.transform.position = bulletFirePoint.position; }

        CharacterControl nextCharacter = nextObject.GetComponent<CharacterControl>();
        nextCharacter.team = team;
        //if (_weaponObject != null) { nextCharacter.InheritWeaponProperties(_weaponObject); }
        //nextCharacter.velocity = velocity.normalized * nextCharacter.moveSpeed;
        //nextCharacter.FrontVelocity(nextCharacter.moveSpeed,ScriptEvent.Operation.ADD);
        nextCharacter.velocity = bulletFirePoint.forward * 0.5f;
        nextCharacter.velocity.z = 0;
        //nextCharacter.UpdatePhysics();
        nextCharacter.FixedUpdate();

        nextCharacter.hitActive = bulletTimeMax;
        //nextCharacter.transform.position += nextObject.transform.up * 0.3f;//nextCharacter.velocity;
        //nextObject.transform.position += nextObject.transform.right * Mathf.Sin(Time.realtimeSinceStartup * fireSinRate) * fireSinePow;
        //if (nextCharacter.mirrorX)
        //{
        //    if (bulletRepeatCount % 2 == 0)
        //    { nextCharacter.mirrorX = true; }
        //    else { nextCharacter.mirrorX = false; }
        //}
        bulletRepeatCount++;
        nextCharacter.VFXEZ(nextCharacter.spawnVFX);
    }

    

    public void DefaultRotation(float _val)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(facingDir, 0, 0), Vector3.up), _val);
    }

    public void FrontVelocityFull(float _val, float _opr, float _stick)
    {
        Vector3 velHelp = transform.forward;
        float frontAngle = Vector3.Angle(Vector3.up, transform.forward);
        float stickAngle = 0;// Vector2.Angle(Vector2.up, new Vector2(stickX, stickY));
        if (CheckStickDeadzone()) { Vector2.Angle(transform.forward, new Vector2(stickX, stickY)); }

        //velHelp.Normalize();
        velHelp *= _val;
        switch (_opr)
        {
            case 0:
                velocity = velHelp;
                break;
            case 1:
                velocity += velHelp;
                break;
            case 2:
                velocity *= _val;  // Vector3.Scale(velocity, velHelp);
                break;
        }
    }
    public void FrontVelocity(float _val, float _opr)
    {
        velHelp = new Vector3(0, 0, 0);
        velHelp = myCharacterTransform.transform.forward;
        velHelp *= _val;//moveSpeed;
        
        switch (_opr)
        {
            case 0:
                //velHelp.y = velocity.y;
                velocity.x = velHelp.x;
                velocity.z = velHelp.z;
                break;
            case 1:
                velocity.x += velHelp.x;
                velocity.z += velHelp.z;
                break;
            case 2:
                velocity *= _val;
                break;
        }
    }
   


    public void UpdateBullet()
    {
        if (bulletTimeMax > 0)
        {
            if (bulletTime < bulletTimeMax) { bulletTime++; }
            else
            {
                if (hitConfirm > 0) { }// { VFX(impactVFX); }
                else { VFXEZ(despawnVFX); }
                Despawn();
            }
        }
    }

    public int hitPause;

    public bool CheckHitList(CharacterControl _target)
    {
        for (int i = 0; i < hitList.Count; i++)
        {
            if (hitList[i] != null)
            {
                if (hitList[i] == _target) { return false; }
            }
        }
        return true;
    }

    public void DodgeCancel()
    {
        if (dodges > 0) { canCancel = true; }
    }

    public void Invincibility(float _len)
    {
        invincibility = (int)_len;
    }

    public void DodgeInvincibility()
    {
        if (dodges > 0) { invincibility = 2; }
    }

    public int curStateLoops = 0;
    public void LoopTo(float _frame, float _loops)
    {
        if (curStateLoops < _loops)
        {
            currentStateFrame = (int)(_frame * currentState.length);
            curStateLoops++;
        }
        else { curStateLoops = 0; }

    }

    public bool driveBreakFlag;
    public float driveBreakPow = 1.5f;

    public float GetDriveDamageBonus()
    {
        float totalDam = 1f;
        totalDam += 0.25f * Mathf.Pow(drive / driveMAX, driveBreakPow);
        totalDam += 1f * Mathf.Pow(overdrive / driveMAX, driveBreakPow);
        totalDam += 5f * Mathf.Pow(driveBreak / driveMAX, driveBreakPow);
        return totalDam;
    }
    public float GetBaseDamageBonus()
    {
        float totalBonus = 100f;
        if (controlType == ControlType.CONTROLLER)
        {
            totalBonus *= GetDriveDamageBonus();
        }
        return totalBonus;
    }

    public int currentAttackIndex = 0;

    public bool InCounterhitWindow()
    {
        for (int i = 0; i < currentState.attacks.Count; i++)
        {
            Attack atk = currentState.attacks[i];
            int attackFrame = (int)(atk.startFrame * currentState.length);
            if (currentStateFrame >= attackFrame - GameEngine.current.counterhitWindow &&
                currentStateFrame <= attackFrame + GameEngine.current.counterhitWindow)
            {
                return true;
            }
        }
        return false;
    }

    public void RandomRotation(float _xRange, float _yRange, float _zRange, float _zFlip)
    {
        transform.Rotate( 0f, 0f, Random.Range(-_zRange, _zRange), Space.Self);
        if (_zFlip > 0) { if (Random.Range(0f, 1f) <= 0.5) { transform.Rotate(0f, 0f, 180f, Space.Self); } }
        transform.Rotate(Random.Range(-_xRange, _xRange) * facingDir, Random.Range(-_yRange, _yRange) * facingDir, 0f ,Space.Self);
        
    }

    
    public static Vector3 RandomVector3()
    {
        return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
    }
    public void GetHit(CharacterControl _attacker)
    {
        if (_attacker == this || _attacker.team == team || controlType == ControlType.BULLET) { return; }
        if (_attacker.currentState.attacks.Count <= 0) { return; }
        if (_attacker.controlType == ControlType.BULLET)
        {
            //if (_attacker.hitConfirm > 0) { return; }
            if (!_attacker.piercing)
            { _attacker.bulletTime = _attacker.bulletTimeMax; }
            //else { _attacker.hitConfirm++; hitActive = 0; }
        }
        if (_attacker.controlType != ControlType.BULLET && _attacker.hitActive <= 0) { return; }


        if (controlType == ControlType.CONTROLLER && invincibility > 0)
        {
            if (GameEngine.current.globalHitpause <= 0) { GameEngine.current.SetHitpause(2); }
        }
        if (_attacker.CheckHitList(this))
        {
            _attacker.hitList.Add(this);
            if (invincibility > 0)
            {
                dodges++;
                return;
            }
            else
            {
                _attacker.hitConfirm++; 
                if (_attacker.hitConfirm == 1)
                {
                    if (_attacker.controlType == ControlType.BULLET)
                    {
                        _attacker.projectileParent.currentCombo++;
                        _attacker.projectileParent.currentComboTimer = GameEngine.current.comboTimerMax;
                        //_attacker.GhostSlash();
                    }
                    else
                    {
                        _attacker.currentCombo++;
                        _attacker.currentComboTimer = GameEngine.current.comboTimerMax;
                        
                    }
                }
                _attacker.hitConfirmed = true;
            }
            if (_attacker.controlType == ControlType.BULLET)
            {
                if (_attacker.piercing) { _attacker.hitPause = 3; }

            }
            Attack atk = _attacker.currentAttack;

            GameEngine.current.SetHitpause(atk.hitPause);

            BasicShake();


            //if (_attacker.controlType == ControlType.BULLET) { VFXEZ(_attacker.projectileWeaponClass?.levels[atk.attackLevel].vfx); }
            //else { VFXEZ(_attacker.currentState?.weaponClass?.levels[atk.attackLevel].vfx); }


            Vector3 nextKnockback = new Vector3();
            nextKnockback.x = atk.knockBack.x;
            nextKnockback.y = atk.knockBack.y;
            //nextKnockback.x *= _attacker.facingDir;// _attacker.transform.forward.x;
            hitPower = atk.hitPower;
            if (hitPower == Attack.HitPower.LIGHT && !atk.ignoreChainKnockback)
            {
                nextKnockback = _attacker.transform.position +
                    (_attacker.transform.forward * 2f) + new Vector3(0f + Random.Range(-0.05f, 0.05f), 0.6f + Random.Range(-0.1f, 0.1f), 0f)
                    - transform.position;

                nextKnockback.Normalize();
                nextKnockback.Scale(new Vector3(0.15f, 0.125f, 0f));

                nextKnockback += _attacker.velocity.normalized * 0.2f;
                nextKnockback.y += 0.1f;
                //knockback *= 0.75f;

            }
            else
            {
                nextKnockback = Quaternion.LookRotation(_attacker.transform.forward) * new Vector3(0f, nextKnockback.y, nextKnockback.x);
                Vector2 perp = Vector2.Perpendicular(new Vector2(nextKnockback.x, nextKnockback.y));
                nextKnockback += new Vector3(perp.x, perp.y, 0) * Random.Range(-0.2f, 0.2f);

            }



            float nextAttackDamage = atk.damage * _attacker.GetBaseDamageBonus();
            if (_attacker.currentState != null && _attacker.controlType != ControlType.BULLET)
            {
                if (_attacker.controlType == ControlType.AI)
                { nextAttackDamage *= _attacker.currentState.dps; }
                if (_attacker.controlType == ControlType.CONTROLLER)
                {
                    //float frameDif = atk.startFrame;
                    //if (_attacker.currentAttackIndex > 0) { frameDif -= _attacker.currentState.attacks[currentAttackIndex - 1].startFrame; }
                    //float frameDif = _attacker.currentState.attacks[0].startFrame;
                    float frameDif = _attacker.currentState.attacks[_attacker.currentState.attacks.Count - 1].cancelWindowStart;
                    nextAttackDamage *= _attacker.currentState.dps * ((frameDif * _attacker.currentState.length) / 60f);
                }
            }

            if (_attacker.controlType == ControlType.CONTROLLER)
            {
                if (_attacker.driveBreak > 0)
                {
                    if (_attacker.hitConfirm == 1)
                    {
                        _attacker.ChangeDrive(_attacker.driveBreak, 0);
                        Debug.Log("DRIVE BREAK : " + _attacker.driveBreak);
                    }
                }
                else
                {
                    float driveBuild = 5f * (1f + (_attacker.overdrive * 0.0025f)) * atk.damage;
                    if (_attacker.hitConfirm > 1) { driveBuild *= 0.05f; }
                    _attacker.ChangeDrive(1f, driveBuild);
                }
                float tpBuild = 0.5f * atk.damage;
                if (_attacker.hitConfirm > 1) { tpBuild *= 0.05f; }
                _attacker.ChangeTP(tpBuild);
            }
            if (controlType == ControlType.CONTROLLER) { ChangeDrive(0f, -overdrive); }


            nextAttackDamage += Random.Range(-1f, 1f) * nextAttackDamage * 0.07f;
            nextAttackDamage = (int)nextAttackDamage;


            int nextHitstun = atk.hitStun;
            nextHitstun += Random.Range(-6, 13);
            //if (nextHitstun < 6) { hitStun = 6; }

            //ARMOR
            if (controlType == ControlType.AI && !atk.guardBreak) //HITSTUN GUARD
            {
                if (currentState != GameEngine.current.hitManager.genericGuard)
                {
                    if (_attacker.currentState.tpCost <= 0 && !_attacker.currentState.bypassHitstunGuard)
                    {
                        hitstunGuardRate = 0.01f;//0.01f; //0.04f
                        hitstunGuardChance += hitstunGuardRate;
                        if (Random.value <= hitstunGuardChance)
                        {
                            StartState(GameEngine.current.hitManager.genericGuard);
                            guardPower = 1;
                            guardReduction = 0.05f;
                            hitstunGuardChance = 0;
                        }
                    }
                }
                else //GUARD CANCEL  GUARDCANCEL
                {
                    guardCancelRate = 0.05f;
                    guardCancelChance += guardCancelRate;
                    if (Random.value <= guardCancelChance)
                    {
                        //Debug.Log("GUARD CANCEL!!!");
                        StartState(GameEngine.current.hitManager.genericGuardCancel);
                        UpdateState();
                        currentAttack = currentState.attacks[0];
                        currentAttackIndex = 0;
                        guardCancelChance = 0;
                        nextHitstun = 0;
                        nextAttackDamage = 0;
                        nextKnockback *= 0;
                        hitstunGuardChance = 0;
                        FaceTargetCharacter(_attacker);
                        _attacker.GetHit(this);
                        _attacker.FaceTargetCharacter(this);
                        ATB = ATBMax;
                        currentJobTime = jobTimeMax;
                    }
                }
            }

            HitResult nextHitResult = HitResult.HIT;

            if (guardPower > 0 || armorCurrent > 0)
            {
                if (!atk.guardBreak) { nextHitResult = HitResult.ARMOR; }
                else { nextHitResult = HitResult.GUARD_CRUSH; }
            }

            if (nextHitResult == HitResult.ARMOR)
            {
                nextAttackDamage *= guardReduction;
                nextKnockback *= 0.1f;
                nextHitstun = 0;
                if (_attacker.hitConfirm == 1)
                {
                    _attacker.velocity += 0.225f * atk.damage * (_attacker.transform.position - transform.position).normalized;
                }
            }
            else
            {
                int nextHitLevel = atk.attackLevel;
                if (atk.hitPower != Attack.HitPower.LIGHT)
                {
                    nextHitLevel = 1;
                    if (_attacker.hitConfirm == 1) { ScreenShakeStandard(); }
                }

                //VFXEZ(_attacker.currentState?.weaponClass?.levels[nextHitLevel].vfx);
                FaceTargetCharacter(_attacker);
            }



            if (nextHitstun > 0)
            {
                if (hitPower == Attack.HitPower.PARRY) { nextHitResult = HitResult.PARRY; }
                if (nextHitResult == HitResult.HIT)
                {
                    if (InCounterhitWindow())
                    {
                        Debug.Log("COUNTERHIT!");
                        nextHitstun += 15;
                        nextAttackDamage *= 2;

                        if (_attacker.hitConfirm == 1)
                        {
                            //GameEngine.current.SetHitpause(15);
                            ScreenShakeJustPow(0.1f);
                        }
                    }

                    if (_attacker.controlType == ControlType.CONTROLLER)
                    {
                        if (_attacker.currentCombo >= 60) { _attacker.currentCombo = 0; nextAttackDamage *= 5; }
                    }
                    if (controlType == ControlType.CONTROLLER) { currentCombo = 0; }
                }

                if (nextHitResult == HitResult.PARRY) { StartState(GameEngine.current.hitManager.genericGuardBreak); }
                else { StartState(GameEngine.current.hitManager.genericHitstun); }


                if (nextHitResult == HitResult.GUARD_CRUSH)
                {
                    StartState(GameEngine.current.hitManager.genericGuardBreak);
                    nextHitstun = 120;
                    nextKnockback *= 0.25f;
                    GameEngine.current.SetHitpause(20);
                    nextAttackDamage *= 0;
                    VFXEZ(GameEngine.current.hitManager.guardBreakVFXPrefab);
                    hitstunGuardChance = 0;
                    guardCancelChance = 0;
                }



                hitStun = nextHitstun;
                hitDir = atk.hitDirection;
                if (hitDir == Attack.HitDirection.HIGH) { hitDirPow = 0.6f; }
                else { hitDirPow = 0.4f; }
                myAnimator.SetFloat("hitDir", hitDirPow);
            }
            if (nextAttackDamage > 0) { Damage(nextAttackDamage); }
            velocity = nextKnockback;

            //StunDamage(atk.stunDamage);
        }
        //}
    }

    public enum HitResult { HIT, ARMOR, GUARD_CRUSH, PARRY, PARRIED, EVADE }

    public void SimpleJump(float _pow, float _opr)
    {
        switch (_opr)
        {
            case 0: velocity.y = _pow; break;
            case 1: velocity.y += _pow; break;
            case 2: velocity.y *= _pow; break;
            case 3: if (velocity.y < 0) { velocity.y *= _pow; } break;
        }
    }
    public void Jump(float _pow, ScriptOperation _opr)
    {
        switch (_opr)
        {
            case ScriptOperation.SET:
                velocity.y = _pow;
                break;
            case ScriptOperation.ADD:
                velocity.y += _pow;
                break;
            case ScriptOperation.MULT:
                velocity.y *= _pow;
                break;
        }
    }

    public enum ScriptOperation { SET, ADD, MULT }
    public void ExecuteEvent(StateEvent _event, int _frame)
    {
        bool startEvent = _frame == _event.start;
        bool endEvent = _frame == _event.end;

        if (_frame >= (int)(_event.start * currentState.length) && _frame <= (int)(_event.end * currentState.length))
        {
            InvokeMethod(_event.script, _event.parameters, _event.prefab);
        }
    }


    public void InvokeMethod(string methodName, List<float> _fParams, GameObject _prefab)
    {

        InvokeMethod(methodName, ConvertToObjectList(_fParams, _prefab));
    }
    public void InvokeMethod(string methodName, List<object> args)
    {
        GetType().GetMethod(methodName).Invoke(this, args.ToArray());
    }

    public List<object> ConvertToObjectList(List<float> _params, GameObject _prefab)
    {
        List<object> newArgs = new List<object>();
        //if (_prefab != null) { newArgs.Add(_prefab); _params.RemoveAt(0); }
        //foreach(float f in _params)
        for (int f = 0; f < _params.Count; f++)
        {
            if (_prefab != null && f == 0) { newArgs.Add(_prefab); }
            else { newArgs.Add(_params[f]); }
        }
        return newArgs;
    }

    public void StunDamage(float _stun)
    {
        if (_stun > stunTime)
        {
            stunTime = _stun;
        }
    }

    public void StartBasicAIAttack()
    {
        currentJobTime = jobTimeMax;
    }
    public void Damage(float _damage)
    {
        //Debug.Log("Damage: " + _damage.ToString() + "--" + health.ToString() + " => " + (health - _damage).ToString());
        //GameEngine.current.damageTextManager.NewDamageText(transform.position, _damage);
        health -= _damage;
        if (health <= 0)
        {
            VFXEZ(despawnVFX);
            if (controlType == ControlType.CONTROLLER) { Respawn(); }
            else { Despawn(); }
        }
    }

    public void Despawn()
    {
        Destroy(gameObject);
    }

    public void VFXEZ(GameObject _vfx)
    {
        VFX(_vfx, 0f, 0f, 0f, 0f, 0f, 0f, 1f, 1f, 1f, 1f, 0f);
    }

    public void VFX(GameObject _vfx,
                     float _posX, float _posY, float _posZ,
                     float _rotX, float _rotY, float _rotZ,
                     float _scaX, float _scaY, float _scaZ,
                     float _spd, float _deparent)
    {
        //if (_scaX == 1) { _scaX = 1f; }
        //if (_scaY == 1) { _scaX = 1f; }
        //if (_scaZ == 1) { _scaX = 1f; }
        if (_spd == 0) { _spd = 1f; }
        if (_vfx != null)
        {
            GameObject nextVFX = Instantiate(_vfx, transform.position, myCharacterTransform.transform.rotation);

            nextVFX.transform.Rotate(new Vector3(_rotX, _rotY, _rotZ));
            //nextVFX.transform.Rotate(new Vector3(0, -90 * facingDir, 0));
            //nextVFX.transform.Rotate(new Vector3(_rotX, 0, 0), Space.Self);
            //nextVFX.transform.Rotate(new Vector3(0, _rotY * facingDir, _rotZ * facingDir), Space.Self);
            Vector3 pos = Quaternion.LookRotation(myCharacterTransform.transform.forward) * new Vector3(_posX, _posY, _posZ);
            nextVFX.transform.position += pos;


            nextVFX.transform.localScale = transform.localScale;
            nextVFX.transform.localScale = Vector3.Scale(nextVFX.transform.localScale, new Vector3(_scaX, _scaY, _scaZ));
            
            nextVFX.transform.SetParent(transform);
            VFXControl fxControl = nextVFX.GetComponent<VFXControl>();
            ParticleSystem.MainModule pMain = fxControl.myParticles.main;
            pMain.simulationSpeed *= _spd;

            fxControl.deparent = _deparent;
            fxControl.myCharacter = this;

            fxControl.myParticles.Simulate(0.016666f, true);
            fxControl.myParticles.Play(true);
            //pMain.= 0.02f;
            //fxControl.startState = currentState;
            //}
        }
    }

    void LateUpdate()
    {
        if (controlType == ControlType.BULLET) { if (ignorePhysics) { myCharacterController.enabled = false; } }
    }
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (controlType == ControlType.BULLET) { ignorePhysics = true; }

        if (hitStun > 0)
        {
            Vector3 refl = Vector3.Reflect(velocity, hit.normal);
            refl.z = 0;
            refl *= 0.9f;
            velocity = refl;

            ignorePhysicsTimer = 1;
        }
        else if ((myCharacterController.collisionFlags & CollisionFlags.Above) != 0)
        {
            if (velocity.y > 0)
            {
                velocity.y *= 0.25f;
            }
        }
    }

    public void ScreenShake(float _pow, float _damp, float _time)
    {
        GameEngine.current.ScreenShake(_pow, _damp, (int)_time);
    }

    public void ScreenShakeJustPow(float _pow)
    {
        GameEngine.current.ScreenShake(_pow, 0.89f, 15);
    }
    public void ScreenShakeStandard()
    {
        GameEngine.current.ScreenShake(0.2f, 0.89f, 15);
    }
    void OnGUI()
    {
        if (controlType == ControlType.CONTROLLER)
        {   //
            //Rect drawRect = new Rect(25, 105, 100, 100);
            //velHelp = new Vector3(stickX, stickY, 0f);
            //if (!CheckStickDeadzone()) { velHelp.x = facingDir; }
            //float targetAngle = Vector3.SignedAngle(transform.forward, velHelp, Vector3.forward);
            //GUI.Label(drawRect, targetAngle.ToString());
            //Rect drawRect = new Rect(20, 10, 100, 100);
            //GUI.Label(drawRect, "HP : " + health.ToString("0.") + "/" + healthMax.ToString("0."));
            //drawRect = new Rect(25, 27, 100, 100);
            //GUI.Label(drawRect, "MP : " + MP.ToString("0.") + "/" + MPMAX.ToString("0."));
            //drawRect = new Rect(25, 44, 100, 100);
            //GUI.Label(drawRect, "TP : " + TP.ToString("0.") + "/" + TPMAX.ToString("0."));


            if (GameEngine.current.drawDebug)
            {
                Rect currentRect = new Rect(00, 30, 100, 100);
                int xSpacing = 30;
                int ySpacing = 15;

                currentRect.y = 20;

                /*
                GUI.Label(currentRect, "LT : " + Input.GetAxisRaw("LT") + "   RT : " + Input.GetAxisRaw("RT"));
                for (int i = 0; i < inputBuffer.buttons.Count; i++)
                {
                    string bName = GameEngine.current.variablesObject.rawInputBindings[i];//inputBuffer.buttons[i].name;
                    
                    currentRect.y = 50;
                    currentRect.x += xSpacing;
                    //bName = bName.Replace("Button", "");
                    //bName = bName.Replace("Stick", "");
                    GUI.Label(currentRect, bName + ":" + inputBuffer.buttons[i].pressOK);
                    for (int b = 0; b < inputBuffer.buttons[i].buffer.Count; b++)
                    {
                        currentRect.y += ySpacing;
                        string iString = inputBuffer.buttons[i].buffer[b].hold.ToString("0.#");

                        if (inputBuffer.buttons[i].buffer[b].press) { iString += ">"; }
                        if (inputBuffer.buttons[i].buffer[b].used) { iString += "--"; }
                        GUI.Label(currentRect, iString);
                    }
                }
                */

            }


        }

        if (controlType == ControlType.AI)
        {
            Rect nextRect = new Rect(0, 0, 200, 200);
            Vector3 drawOffset = new Vector3(0, -0.5f, 0);
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + drawOffset);
            //screenPosition.y = Screen.height - screenPosition.y;
            //
            //
            //nextRect.position = new Vector2(screenPosition.x, screenPosition.y);
            //GUI.Label(nextRect, currentState.name + " : " + currentStateFrame);
            //
            //
            //
            //nextRect = new Rect(0, 0, 200, 200);
            //drawOffset = new Vector3(0, -0.25f, 0);
            //screenPosition = Camera.main.WorldToScreenPoint(transform.position + drawOffset);
            //screenPosition.y = Screen.height - screenPosition.y;
            //nextRect.position = new Vector2(screenPosition.x, screenPosition.y);
            //GUI.Label(nextRect, "HP : " + health.ToString("0.") + "/" + healthMax.ToString("0."));
            ////nextRect.position = new Vector2(screenPosition.x, screenPosition.y);
            ////GUI.Label(nextRect, "Hit Pause" + " : " + GetVariable(hitPause).ToString());
            //
            //nextRect = new Rect(0, 0, 200, 200);
            //drawOffset = new Vector3(0, -0.75f, 0);
            //screenPosition = Camera.main.WorldToScreenPoint(transform.position + drawOffset);
            //screenPosition.y = Screen.height - screenPosition.y;
            //
            //nextRect.position = new Vector2(screenPosition.x, screenPosition.y);
            //GUI.Label(nextRect, "Hit Stun" + " : " + hitStun.ToString());

            nextRect = new Rect(0, 0, 200, 200);
            drawOffset = new Vector3(-0.5f, -0.5f, 0);
            screenPosition = Camera.main.WorldToScreenPoint(transform.position + drawOffset);
            screenPosition.y = Screen.height - screenPosition.y;
            nextRect.position = new Vector2(screenPosition.x, screenPosition.y);
            GUI.Label(nextRect, "ATB:" + ATB.ToString("000."));
        }

    }

}
