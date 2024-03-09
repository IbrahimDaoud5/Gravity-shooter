using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class MoveToGoal : Agent
{
    public override void OnActionReceived(ActionBuffers actions)
    {
        Debug.Log(actions.ContinuousActions[0]);
    }
}
