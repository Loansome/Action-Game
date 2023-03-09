using UnityEngine;
using System.Collections;

public class DynamicCameraControl : MonoBehaviour
{
    [HideInInspector]
    public string horizontalAxis = "RightStickX";  //As Defined in your Unity Editor: Edit > Project Settings > Input
    [HideInInspector]
    public string verticalAxis = "RightStickY";
    public GameObject rig;

    public enum CameraTargets { PLAYER, TARGET, BATTLE_CENTER }
    public CameraTargets transformTarget;
    public CameraTargets lookTarget;
    public GameObject transformTargetObject;
    public GameObject lookTargetObject;
    public Vector3 transformOffset;
    public GameObject myCamera;
    public float orbH;
    public float orbV;
    public float orbVMin = -89f;
    public float orbVMax = 89f;
    public float rotSpeed = 2;
    public float rotBoost = 0f;
    public float rotAccel = 0.075f;
    public float rotBoostMax = 5;
    public float orbSpeed = 0.5f;
    public float targetOrbH;
    public float targetOrbV;
    public float orbitHelp = 1.05f;

    public int currentCamDistanceIndex;
    public float[] camDistances = new float[3] { 10f, 15f, 7f };
    public float camDistance = 8f;

    public float deadzone = 0.5f;
    public float invertX = 1;
    public float invertY = 1;

    public Vector3 translateSpeed = new Vector3(0.75f, 0.075f, 0.75f); //X and Z should be the same generally as this is the horizontal plane

    public bool focusTarget;

    public Vector3 lookEuler;

    public bool ignoreTranslate;
    public bool ignoreRotate;

    void Awake()
    {
        //DontDestroyOnLoad(transform.gameObject);
    }

    // Use this for initialization
    void Start()
    {
        //Delete this line and add your own code or assign in editor
        //transformTargetObject = CitadelDeep.GetMainCharacterObject();
        //transformTargetObject = GameEngine.globalArenaBounds;
    }



    void SettleCameras()
    {
        myCamera.transform.localPosition += (Vector3.zero - myCamera.transform.localPosition) * 0.25f;
    }

    void Translate()
    {
        if (!ignoreTranslate)
        {

            if (transformTargetObject == null) { return; }
            transform.position += Vector3.Scale((transformTargetObject.transform.position + (Quaternion.LookRotation(new Vector3(transform.forward.x, 0f, transform.forward.z), Vector3.up) * transformOffset) - transform.position), translateSpeed);
        }
    }

    void OrbitView()
    {
        if (Input.GetAxisRaw(horizontalAxis) > deadzone)
        {
            targetOrbH += (rotBoost + rotSpeed) * invertX;
            rotBoost += rotAccel;
        }
        if (Input.GetAxisRaw(horizontalAxis) < -deadzone)
        {
            targetOrbH += -(rotBoost + rotSpeed) * invertX;
            rotBoost += rotAccel;
        }
        if (Input.GetAxisRaw(verticalAxis) > deadzone) //DOWN
        {
            targetOrbV += (rotBoost + rotSpeed) * invertY;
            rotBoost += rotAccel;
        }
        if (Input.GetAxisRaw(verticalAxis) < -deadzone) //UP
        {
            targetOrbV += -(rotBoost + rotSpeed) * invertY;
            rotBoost += rotAccel;
        }

        if (Input.GetAxisRaw(horizontalAxis) < deadzone &&
           Input.GetAxisRaw(horizontalAxis) > -deadzone &&
           Input.GetAxisRaw(verticalAxis) < deadzone &&
           Input.GetAxisRaw(verticalAxis) > -deadzone)
        {
            rotBoost = 0;
        }
        if (rotBoost > rotBoostMax) { rotBoost = rotBoostMax; }

        targetOrbV = Mathf.Clamp(targetOrbV, orbVMin, orbVMax);

        if (!focusTarget) { targetOrbH -= LookAtOffset() * orbitHelp * Mathf.Lerp(1f, 0.025f, Mathf.InverseLerp(0f, 90f, Mathf.Abs(orbV))); }  // * 180f / Mathf.PI;


        //targetOrbH = Mathf.Repeat(targetOrbH, 360);
        //Ease orbiting
        orbH += (targetOrbH - orbH) * orbSpeed;
        orbV += (targetOrbV - orbV) * orbSpeed;

        // targetOrbH = WrapAngle(targetOrbH);
        //orbH = WrapAngle(orbH);

        if (!ignoreRotate)
        {
            switch (lookTarget)
            {
                case CameraTargets.PLAYER: transform.rotation = Quaternion.Euler(orbV, orbH, 0); break;
                case CameraTargets.TARGET: transform.rotation = Quaternion.LookRotation(lookTargetObject.transform.position - transform.position, Vector3.up) * Quaternion.Euler(orbV, 0, 0); break;
            }


            if (focusTarget) { transform.rotation = Quaternion.LookRotation(lookTargetObject.transform.position - transform.position, Vector3.up) * Quaternion.Euler(orbV, 0, 0); }
        }
    }

    public float WrapAngle(float angle)
    {
        if (angle < 0f) { return angle + 360f; }
        else if (angle > 360f) { return angle - 360f; }

        return angle;
    }

    public float LookAtOffset()
    {
        if (transformTargetObject == null) { return 0f; }
        if (rig == null) { return 0f; }
        float lookAtOffset = 0;

        Vector3 offsetLook = Quaternion.LookRotation(new Vector3(transform.forward.x, 0f, transform.forward.z), Vector3.up) * transformOffset;
        Vector2 currentLook = new Vector2(transform.position.x, transform.position.z) - new Vector2(rig.transform.position.x, rig.transform.position.z) - new Vector2(offsetLook.x, offsetLook.z);
        Vector2 newLook = new Vector2(transformTargetObject.transform.position.x, transformTargetObject.transform.position.z) - new Vector2(rig.transform.position.x, rig.transform.position.z);
        Vector3 cross = Vector3.Cross(currentLook, newLook);
        lookAtOffset = Vector2.Angle(currentLook, newLook);
        if (cross.z > 0) { return lookAtOffset; }
        else { return -lookAtOffset; }
    }


    //Camera Collision Code adapted from https://www.youtube.com/watch?v=7BcxyHi4Jwo Renaissance Coders

    public LayerMask collisionLayer;
    [HideInInspector]
    public Vector3[] cameraClipPoints = new Vector3[5];

    public float collisionBuffer = 3.41f;

    public void UpdateCameraClipPoints()
    {
        //cameraClipPoints = new Vector3[5];

        float z = Camera.main.nearClipPlane;
        float x = Mathf.Tan(Camera.main.fieldOfView / collisionBuffer) * z;
        float y = x / Camera.main.aspect;

        cameraClipPoints[0] = (Camera.main.transform.rotation * new Vector3(-x, y, z)) + Camera.main.transform.position;
        cameraClipPoints[1] = (Camera.main.transform.rotation * new Vector3(x, y, z)) + Camera.main.transform.position;
        cameraClipPoints[2] = (Camera.main.transform.rotation * new Vector3(-x, -y, z)) + Camera.main.transform.position;
        cameraClipPoints[3] = (Camera.main.transform.rotation * new Vector3(x, -y, z)) + Camera.main.transform.position;

        cameraClipPoints[4] = Camera.main.transform.position - Camera.main.transform.forward;
    }

    public float CollisionDistance()
    {
        for (int i = 0; i < cameraClipPoints.Length; i++)
        {
            Ray ray = new Ray(transformTargetObject.transform.position, cameraClipPoints[i] - transformTargetObject.transform.position);
            float cDistance = Vector3.Distance(cameraClipPoints[i], transformTargetObject.transform.position);
            if (Physics.Raycast(ray, cDistance, collisionLayer))
            { return GetAdjustedDistance(transformTargetObject.transform.position); }
        }
        return camDistance;
    }

    public float GetAdjustedDistance(Vector3 from)
    {
        float distance = -1;

        for (int i = 0; i < cameraClipPoints.Length; i++)
        {
            Ray ray = new Ray(from, cameraClipPoints[i] - from);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (distance == -1) { distance = hit.distance; }
                else if (hit.distance < distance) { distance = hit.distance; }
            }
        }

        if (distance == -1) { return camDistance; }
        return distance;
    }



    public float collisionDistance;

    // Update is called once per frame
    void FixedUpdate()//LateUpdate()
    {

        UpdateCamDistance();

        /*
        switch (transformTarget)
        {
            case CameraTargets.PLAYER: transformTargetObject = CitadelDeep.GetMainCharacterObject(); break;
            case CameraTargets.TARGET: transformTargetObject = CitadelDeep.GetCharacterObject(CitadelDeep.GetMainCharacterScript().attackTargetIndex); break;
            case CameraTargets.BATTLE_CENTER: transformTargetObject = GameEngine.globalArenaBounds; break;
        }

        if (transformTargetObject == null) { transformTargetObject = CitadelDeep.GetMainCharacterObject(); }

        switch (lookTarget)
        {
            case CameraTargets.PLAYER: lookTargetObject = CitadelDeep.GetMainCharacterObject(); break;
            case CameraTargets.TARGET: lookTargetObject = CitadelDeep.GetCharacterObject(CitadelDeep.GetMainCharacterScript().attackTargetIndex); break;
            case CameraTargets.BATTLE_CENTER: lookTargetObject = GameEngine.globalArenaBounds; break;
        }

        if (transformTargetObject == null) { transformTargetObject = CitadelDeep.GetMainCharacterObject(); }
        */


        rig.transform.localPosition = new Vector3(0, 0, -camDistance);
        OrbitView();
        Translate();
        SettleCameras();

        UpdateCameraClipPoints();
        collisionDistance = CollisionDistance();
        if (collisionDistance > camDistance) { collisionDistance = camDistance; }
        rig.transform.localPosition = new Vector3(0, 0, -collisionDistance);
    }

    public void UpdateCamDistance()
    {
        //if (Input.GetButtonDown("R3")) { currentCamDistanceIndex++; }
        if (currentCamDistanceIndex > camDistances.Length - 1) { currentCamDistanceIndex = 0; }
        camDistance = camDistances[currentCamDistanceIndex];
    }
}
