using UnityEngine;
using MLAgents;

public class HunterAcademy : Academy
{
    public float agentRunSpeed = 1.5F;
    public float agentRotationSpeed = 1F;
    public float gravityMultiplier = 3F; // use ~3 to make things less floaty

    public override void InitializeAcademy()
    {
        Physics.gravity *= gravityMultiplier;
    }

    public override void AcademyReset()
    {
    }
}
