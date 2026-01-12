using UnityEngine;

public class RepeatBtNode : DecoratorBtNode
{
    public int loopCnt;
    public bool infinityLoop = false;

    private int nowLoopCnt = 0;
    protected override void OnStart()
    {
        nowLoopCnt = 0;
    }

    protected override BTNodeState OnUpdate()
    {
        if (!infinityLoop)
        {
            if (nowLoopCnt < loopCnt)
            {
                var state = ChildrensList[0].Update();
                if (state != BTNodeState.Running) 
                    nowLoopCnt++;
                return BTNodeState.Running;
            }
            else
            {
                return BTNodeState.Success;
            }
        }
        else
        {
            ChildrensList[0].Update();
            return BTNodeState.Running;
        }
    }

    protected override void OnStop()
    {
    }
}
