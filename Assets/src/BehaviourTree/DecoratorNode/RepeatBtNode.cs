using UnityEngine;

public class RepeatBtNode : DecoratorBtNode
{
    public int loopCnt;
    protected override void OnStart()
    {
        
    }

    protected override State OnUpdate()
    {
        child.Update();
        return State.Running;
    }

    protected override void OnStop()
    {
    }
}
