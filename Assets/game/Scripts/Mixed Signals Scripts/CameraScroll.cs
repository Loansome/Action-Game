using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScroll : MonoBehaviour
{

    private GameObject player;
    public float xMin, xMax, yMin, yMax, followSpeed;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        float x = Mathf.Clamp(player.transform.position.x, xMin, xMax);
        float y = Mathf.Clamp(player.transform.position.y, yMin, yMax);
        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, new Vector3(x, y, gameObject.transform.position.z), followSpeed * Time.deltaTime);
    }
}
