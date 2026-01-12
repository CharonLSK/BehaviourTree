using UnityEngine;
public abstract class DecoratorBtNode : BTNode
{
    public override AllowedChildCount ChildCntType => AllowedChildCount.Single;
    
    public override BTNode Clone()
    {
        DecoratorBtNode btNode = Instantiate(this);
        btNode.ChildrensList.ConvertAll(child => {return child.Clone();});
        return btNode;
    }

}
