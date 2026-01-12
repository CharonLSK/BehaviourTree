using System.Collections.Generic;
using UnityEngine;

public abstract class CompositeBtNode : BTNode
{
    public override AllowedChildCount ChildCntType => AllowedChildCount.Multiple;
    
    public override BTNode Clone()
    {
        CompositeBtNode btNode = Instantiate(this);

        btNode.ChildrensList = ChildrensList.ConvertAll(c => c.Clone());
        return btNode;
    }

}
