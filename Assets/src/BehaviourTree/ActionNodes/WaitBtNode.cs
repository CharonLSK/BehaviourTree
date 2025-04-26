using UnityEngine;

public class WaitBtNode : ActionBtNode
{
    public float duration = 1;
    private float startTime;

    protected override void OnStart()
    {
        startTime = Time.time;
    }

    protected override State OnUpdate()
    {
        if (Time.time - startTime > duration)
        {
            return State.Success;
        }

        return State.Running;

    }

    protected override void OnStop()
    {
    }
}
