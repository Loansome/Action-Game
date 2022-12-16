using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    
    public Transform objectFollow;
    [SerializeField] float followSpeed;

    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, objectFollow.transform.position, followSpeed * Time.deltaTime);
    }
}
