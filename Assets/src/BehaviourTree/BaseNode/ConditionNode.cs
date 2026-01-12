using UnityEngine;

public abstract class ConditionNode : BTNode
{

    public BT_BbDataBase.BTValCompType CompType;
    
    protected abstract bool Check();
    public override AllowedChildCount ChildCntType
    {
        get { return AllowedChildCount.None; }
    }

    protected override BTNodeState OnUpdate()
    {
        return (Check()) ? BTNodeState.Success : BTNodeState.Failure;
    }

    public override BTNode Clone()
    {
        ConditionNode btNode = Instantiate(this);
        btNode.ChildrensList = ChildrensList.ConvertAll(c => c.Clone());
        return btNode;
    }
}
