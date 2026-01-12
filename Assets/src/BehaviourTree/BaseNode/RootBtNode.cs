
public class RootBtNode : BTNode
{
    public override AllowedChildCount ChildCntType => AllowedChildCount.Single;

    protected override void OnStart()
    {
        
    }
    protected override BTNodeState OnUpdate()
    {
        return ChildrensList[0].Update();
    }
    protected override void OnStop()
    {
        
    }

    public override BTNode Clone()
    {
        RootBtNode btNode = Instantiate(this);
        btNode.ChildrensList = ChildrensList.ConvertAll(child => { return child.Clone(); });
        return btNode;
    }
}
