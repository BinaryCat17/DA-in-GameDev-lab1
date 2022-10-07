using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RollerAgent2 : Agent
{
    Rigidbody rBody;
    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public GameObject Target1;
    private bool active1 = false;
    public GameObject Target2;
    private bool active2 = false;
    private float revard = 0.0f;

    public override void OnEpisodeBegin()
    {
        if (this.transform.localPosition.y < 0)
        {
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0, 0.5f, 0);
        }

        Target1.transform.localPosition = new Vector3(Random.value * 8-4, 0.5f, Random.value * 8-4);
        Target2.transform.localPosition = new Vector3(Random.value * 8-4, 0.5f, Random.value * 8-4);

        revard = 0.0f;
        active1 = true;
        active2 = true;
        Target1.SetActive(true);
        Target2.SetActive(true);
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(Target1.transform.localPosition);
        sensor.AddObservation(Target2.transform.localPosition);
        sensor.AddObservation(active1);
        sensor.AddObservation(active2);
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
    }
    public float forceMultiplier = 10;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        rBody.AddForce(controlSignal * forceMultiplier);

        float distanceToTarget1 = Vector3.Distance(this.transform.localPosition, Target1.transform.localPosition);
        float distanceToTarget2 = Vector3.Distance(this.transform.localPosition, Target2.transform.localPosition);


        if(active1 && distanceToTarget1 < 1.42f) {
            active1 = false;
            revard += 0.5f;
            Target1.SetActive(false);
        }

        if(active2 && distanceToTarget2 < 1.42f) {
            active2 = false;
            revard += 0.5f;
            Target2.SetActive(false);
        }

        if(this.transform.localPosition.y < 0) {
            EndEpisode();
        } else if((!active1 && !active2)) {
            SetReward(revard);
            EndEpisode();
        }
    }
}
