using UnityEngine;

public class SelectorNode : CompositeBtNode
{
    private int currentIdx;
    protected override void OnStart()
    {
        currentIdx = 0;
    }

    protected override BTNodeState OnUpdate()
    {
        if (ChildrensList.Count == 0)
        {
            return BTNodeState.Failure;
        }
        var child = ChildrensList[currentIdx];
        var childNodeState = child.Update();
        switch (childNodeState)
        {
            case BTNodeState.Running:
                return BTNodeState.Running;
            case BTNodeState.Failure:
                currentIdx++;
                break;
            case BTNodeState.Success:
                return BTNodeState.Success;
        }
        return currentIdx == ChildrensList.Count ? BTNodeState.Failure : BTNodeState.Running;
    }

    protected override void OnStop()
    {
        
    }

}
