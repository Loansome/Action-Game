using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentedSimulation : MonoBehaviour
{

    public Vector3 gravity;
    public float dampen = 0.95f;
    public float progDampen = 0.95f;
    public int simulations = 50;
    public Vector3 twistFix;

    //public PonytailSegment rootSegment;
    public List<PhysicsSegment> segments;

    // Start is called before the first frame update
    void Start()
    {
        //segments.Add(rootSegment);
        foreach (PhysicsSegment pS in GetComponentsInChildren<PhysicsSegment>())
        {
            segments.Add(pS);
            pS.transform.parent = transform;
            //pS.transform.parent = null;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    //void FixedUpdate()
    {
        //if (GameEngine.current.sceneTransitioner.inTransition) { return; }
        Simulate();
    }

    public void ApplyForce(Vector3 _force, float _decay)
    {
        currentForce = _force;
        forceDecay = _decay;
    }

    public Vector3 currentForce;
    public float forceDecay;
    public void Simulate()
    {
        float curDamp = dampen;
        for (int i = 1; i < segments.Count; i++)
        {
            PhysicsSegment firstSegment = segments[i];
            //Vector3 offset = (firstSegment.posNow - firstSegment.posOld).normalized;
            Vector3 velocity = (firstSegment.posNow - firstSegment.posOld) * curDamp;
            firstSegment.posOld = firstSegment.posNow;
            firstSegment.posNow += velocity;
            firstSegment.posNow += currentForce;
            firstSegment.posNow += gravity; //* curDamp
            firstSegment.transform.position = firstSegment.posNow;
            segments[i] = firstSegment;

            curDamp *= progDampen;
        }
        currentForce *= forceDecay;
        //CONSTRAINTS
        for (int i = 0; i < simulations; i++)
        {
            ApplyConstraint();
        }
        for (int i = 0; i < segments.Count - 1; i++)
        {
            PhysicsSegment firstSeg = segments[i];
            PhysicsSegment secondSeg = segments[i + 1];

            secondSeg.transform.LookAt(firstSeg.transform);//, -transform.parent.up); //segments[0].transform.right);//, Vector3.up);// transform.parent.forward);
            if (i == 0) { firstSeg.transform.Rotate(firstSeg.twistFix, Space.Self); }
            secondSeg.transform.Rotate(secondSeg.twistFix, Space.Self);
        }

    }

    public float ropeSegLen = 2f;
    private void ApplyConstraint()
    {
        //Constrant to Mouse
        PhysicsSegment firstSegment = segments[0];
        firstSegment.posNow = firstSegment.transform.position;//Camera.main.ScreenToWorldPoint(Input.mousePosition);
        segments[0] = firstSegment;

        for (int i = 0; i < segments.Count - 1; i++)
        {
            PhysicsSegment firstSeg = segments[i];
            PhysicsSegment secondSeg = segments[i + 1];

            float dist = (firstSeg.posNow - secondSeg.posNow).magnitude;
            float error = Mathf.Abs(dist - this.ropeSegLen);
            Vector3 changeDir = Vector3.zero;

            if (dist > ropeSegLen)
            {
                changeDir = (firstSeg.posNow - secondSeg.posNow).normalized;
            }
            //else if (dist < ropeSegLen)
            //{
            //    changeDir = (secondSeg.posNow - firstSeg.posNow).normalized;
            //}

            Vector3 changeAmount = changeDir * error;
            if (i != 0)
            {
                firstSeg.posNow -= changeAmount * 0.5f;
                segments[i] = firstSeg;
                secondSeg.posNow += changeAmount * 0.5f;
                segments[i + 1] = secondSeg;
            }
            else
            {
                secondSeg.posNow += changeAmount;
                segments[i + 1] = secondSeg;
            }
            //secondSeg.transform.LookAt(firstSeg.transform);
            //secondSeg.transform.Rotate(-secondSeg.transform.up * 90f);
            //transform.LookAt(transform.parent);
            //transform.Rotate(transform.up * 90);
        }
    }


}
