
public class RootBtNode : BTNode
{
    public BTNode child;
    protected override void OnStart()
    {
        
    }

    protected override State OnUpdate()
    {
        return child.Update();
    }

    protected override void OnStop()
    {
    }

    public override BTNode Clone()
    {
        RootBtNode btNode = Instantiate(this);
        btNode.child = child.Clone();
        return btNode;
    }
}
