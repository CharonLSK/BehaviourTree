using UnityEngine;

public class DebugLogBtNode : ActionBtNode
{
    public string message;
    protected override void OnStart()
    {
        Debug.Log($"onStart: {message}");
    }

    protected override State OnUpdate()
    {
        Debug.Log($"OnUpdate: {message}");
        return State.Success;
    }

    protected override void OnStop()
    {
        Debug.Log($"OnStop: {message}");
    }
}
