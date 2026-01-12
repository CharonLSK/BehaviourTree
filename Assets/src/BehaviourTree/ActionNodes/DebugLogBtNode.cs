using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DebugLogBtNode : ActionBtNode
{
    public string dicName;
    public string message;
    protected override void OnStart()
    {
        message = btBlackboard.GetValue<BtStringVal>(dicName).value;
    }

    protected override BTNodeState OnUpdate()
    {
        Debug.Log($"OnUpdate: {message}");
        return BTNodeState.Success;
    }

    protected override void OnStop()
    {
    }
}
