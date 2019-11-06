using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class HunterAgent : Agent
{
    public Transform Prey;

    Rigidbody p_rBody;
    Rigidbody h_rBody;
    HunterAcademy m_Academy;
    public override void InitializeAgent()
    {
        base.InitializeAgent();
        m_Academy = FindObjectOfType<HunterAcademy>();
        h_rBody = GetComponent<Rigidbody>();
        p_rBody = Prey.GetComponent<Rigidbody>();
    }

    public override void AgentReset()
    {
        if (this.transform.localPosition.x > 10 || this.transform.localPosition.x < -10
            || this.transform.localPosition.z < -10 || this.transform.localPosition.z > 10
            || this.transform.localPosition.y < 0 || this.transform.localPosition.y > 1)
        {
            // If the Agent went out of bounds, zero its momentum
            this.h_rBody.angularVelocity = Vector3.zero;
            this.h_rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0, 0.5f, 0);
        }
        
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Prey.localPosition);

        // Move the prey to a new spot
        if (distanceToTarget < 1.42f)
        {
            p_rBody.angularVelocity = Vector3.zero;
            p_rBody.velocity = Vector3.zero;
            Prey.localPosition = new Vector3(
                Random.value * 8 - 4,
                0.5f,
                Random.value * 8 - 4
            );
        }
    }

    public override void CollectObservations()
    {
        // Prey and Agent positions
        AddVectorObs(Prey.localPosition);
        AddVectorObs(this.transform.localPosition);
        AddVectorObs(this.transform.localRotation.y);
        // Agent velocity
        AddVectorObs(h_rBody.velocity.x);
        AddVectorObs(h_rBody.velocity.z);
    }

    public void MoveAgent(float[] act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        

        if (brain.brainParameters.vectorActionSpaceType == SpaceType.Continuous)
        {
            //dirToGo = transform.forward * Mathf.Clamp(act[0], -1f, 1f);
            dirToGo = 2 * transform.forward * Mathf.Clamp(act[0], -0.8f, 1f);
            rotateDir = transform.up * Mathf.Clamp(act[1], -1f, 1f);
        }
        else
        {
            var action = Mathf.FloorToInt(act[0]);
            switch (action)
            {
                case 1:
                    dirToGo = transform.forward * 1f;
                    break;
                case 2:
                    dirToGo = transform.forward * -1f;
                    break;
                case 3:
                    rotateDir = transform.up * 1f;
                    break;
                case 4:
                    rotateDir = transform.up * -1f;
                    break;
            }
        }
        transform.Rotate(rotateDir, Time.deltaTime * 150f);
        h_rBody.AddForce(dirToGo * m_Academy.agentRunSpeed, ForceMode.VelocityChange);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        AddReward(-1f / 1000);
        MoveAgent(vectorAction);

        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Prey.localPosition);

        // Prey caught
        if (distanceToTarget < 1.42f)
        {
            AddReward(1.0f);
            Done();
        }

        // Out of bounds
        if (this.transform.localPosition.x > 10 || this.transform.localPosition.x < -10
            || this.transform.localPosition.z < -10 || this.transform.localPosition.z > 10
            || this.transform.localPosition.y < 0 || this.transform.localPosition.y > 1)
        {
            Done();
        }
    }
}