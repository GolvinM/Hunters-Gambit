using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class PreyAgent : Agent
{
    Rigidbody rBody;
    HunterAcademy m_Academy;
    public override void InitializeAgent()
    {
        base.InitializeAgent();
        m_Academy = FindObjectOfType<HunterAcademy>();
        rBody = GetComponent<Rigidbody>();
    }

    public Transform Hunter;
    public override void AgentReset()
    {

        if (this.transform.localPosition.x > 10 || this.transform.localPosition.x < -10
            || this.transform.localPosition.z < -10 || this.transform.localPosition.z > 10
            || this.transform.localPosition.y < 0 || this.transform.localPosition.y > 1)
        {
            // If the Agent went out of bounds, zero its momentum
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(
                Random.value * 8 - 4,
                0.5f,
                Random.value * 8 - 4
            );
        }
    }

    public override void CollectObservations()
    {
        // Hunter and Agent positions
        AddVectorObs(Hunter.localPosition);
        AddVectorObs(this.transform.localPosition);
        AddVectorObs(this.transform.localRotation.y);

        // Agent velocity
        AddVectorObs(rBody.velocity.x);
        AddVectorObs(rBody.velocity.z);
    }

    public void MoveAgent(float[] act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        if (brain.brainParameters.vectorActionSpaceType == SpaceType.Continuous)
        {
            //dirToGo = transform.forward * Mathf.Clamp(act[0], -1f, 1f);
            dirToGo = transform.forward * Mathf.Clamp(act[0], -0.8f, 1f);
            rotateDir = 2 * transform.up * Mathf.Clamp(act[1], -1f, 1f);
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
        rBody.AddForce(dirToGo * m_Academy.agentRunSpeed, ForceMode.VelocityChange);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        AddReward(+1f / 100);
        MoveAgent(vectorAction);

        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Hunter.localPosition);

        // Prey caught
        if (distanceToTarget < 1.42f)
        {
            AddReward(-1.0f);
            MyDone();
        }

        // Out of bounds
        if(this.transform.localPosition.x > 10 || this.transform.localPosition.x < -10 
            || this.transform.localPosition.z < -10 || this.transform.localPosition.z > 10
            || this.transform.localPosition.y < 0 || this.transform.localPosition.y > 1)
        {
            Done();
        }
    }

    public void MyDone()
    {
        Debug.Log("Reward: " + GetCumulativeReward().ToString() + "\n" +
            "MaxStep: " + agentParameters.maxStep);

        Done();
    }
}