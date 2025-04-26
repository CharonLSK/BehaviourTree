using System.Collections.Generic;
using UnityEngine;

public abstract class CompositeBtNode : BTNode
{ 
    public List<BTNode> children = new List<BTNode>();
    
    public override BTNode Clone()
    {
        CompositeBtNode btNode = Instantiate(this);

        btNode.children = children.ConvertAll(c => c.Clone());
        return btNode;
    }

}
