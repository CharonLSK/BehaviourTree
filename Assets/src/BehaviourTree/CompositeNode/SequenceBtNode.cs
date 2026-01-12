using UnityEngine;

public class SequenceBtNode : CompositeBtNode
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
               return BTNodeState.Failure;
           case BTNodeState.Success:
               currentIdx++;
               break;
        }
        return currentIdx == ChildrensList.Count ? BTNodeState.Success : BTNodeState.Running;
    }

    protected override void OnStop()
    {
    }
}
