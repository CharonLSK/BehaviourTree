using UnityEngine;

public class WaitBtNode : ActionBtNode
{
    public float duration = 1;
    private float startTime;

    protected override void OnStart()
    {
        startTime = Time.time;
    }

    protected override BTNodeState OnUpdate()
    {
        if (Time.time - startTime > duration)
        {
            return BTNodeState.Success;
        }
        return BTNodeState.Running;
    }

    protected override void OnStop()
    {
    }
}
