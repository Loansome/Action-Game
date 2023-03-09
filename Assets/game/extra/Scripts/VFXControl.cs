using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXControl : MonoBehaviour
{
    public ParticleSystem myParticles;
    public float deparent;
    public CharacterControl myCharacter;
    //public State startState;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (myParticles.time > myParticles.main.duration) { Despawn(); }
        //if (myParticles.time >= deparent || myCharacter.currentState != startState) { if (transform.parent != null) { transform.parent = null; } }
        if (myParticles.time >= deparent) { Deparent(); }
        if (!myParticles.isPlaying) { Despawn(); }
    }

    public void Deparent()
    {
        if (transform.parent != null) { transform.parent = null; }
    }
    void Despawn()
    {
        Destroy(gameObject);
    }
}
