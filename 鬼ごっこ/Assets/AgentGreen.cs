using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;
//緑エージェント
public class AgentGreen : AgentBase
{
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(transform.forward);
        sensor.AddObservation(otherAgents[0].transform.localPosition);
        sensor.AddObservation(otherAgents[0].transform.forward);
        sensor.AddObservation(otherAgents[1].transform.localPosition);
        sensor.AddObservation(otherAgents[1].transform.forward);
    }
    public void MoveAgent(ActionSegment<int> act)
    {
        var rotateDir = Vector3.zero;
        var rotateAxis = act[0];
        switch (rotateAxis)
        {
            case 1:
                rotateDir = transform.up * -1f;
                break;
            case 2:
                rotateDir = transform.up * 1f;
                break;
        }
        transform.Rotate(rotateDir, Time.deltaTime * 100f * angularVelocityCoefficient);
        selfRidigBody.velocity = Vector3.zero;
        selfRidigBody.AddForce(transform.forward * .1f * velocityCoefficient);
    }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        MoveAgent(actionBuffers.DiscreteActions);
        if (hasTouchedChild)
        {
            SetReward(1f);
            EndEpisode();
        }
        if (hasTouchedDemon)
        {
            SetReward(-1f);
            EndEpisode();
        }
    }
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("AgentRed"))
        {
            hasTouchedDemon = true;
        }
        if (other.gameObject.CompareTag("AgentBlue"))
        {
            hasTouchedChild = true;
        }
 
    }

}
