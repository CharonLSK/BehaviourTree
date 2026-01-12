using UnityEditor.Animations;
using UnityEngine;

public class ChangeAnimNode : ActionBtNode
{
    public string triggerName;
    private Animator control;

    public override void Active(GameObject obj)
    {
        base.Active(obj);
        control = belongGameobj.GetComponent<Animator>();
    }

    protected override void OnStart()
    {
        control.SetTrigger(triggerName);
    }

    protected override BTNodeState OnUpdate()
    {
        return BTNodeState.Success;
    }

    protected override void OnStop()
    {
    }
}
