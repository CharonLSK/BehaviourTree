using UnityEngine;

public class SequenceBtNode : CompositeBtNode
{
    private int current;
    protected override void OnStart()
    {
        current = 0;
    }

    protected override State OnUpdate()
    {
        if (children.Count == 0)
        {
            return State.Failure;
        }
        var child = children[current];
        switch (child.Update())
        {
           case State.Running:
               return State.Running;
           case State.Failure:
               return State.Failure;
           case State.Success:
               current++;
               break;
        }
        return current == children.Count ? State.Success : State.Running;
    }

    protected override void OnStop()
    {
    }
}
