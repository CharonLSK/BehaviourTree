using UnityEngine;
public abstract class DecoratorBtNode : BTNode
{
    public BTNode child;
    
    public override BTNode Clone()
    {
        DecoratorBtNode btNode = Instantiate(this);
        btNode.child = child.Clone();
        return btNode;
    }

}
