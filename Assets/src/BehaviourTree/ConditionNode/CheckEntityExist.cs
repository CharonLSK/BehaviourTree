
using UnityEngine;

public class CheckEntityExist: ConditionNode
{
    [BT_BBAttr(BT_BbDataBase.BTValType.tString)]
    public string entityValName;

    private BtStringVal nameSO;

    public override void Active(GameObject obj)
    {
        base.Active(obj);
        nameSO = btBlackboard.GetValue<BtStringVal>(entityValName);
    }

    public override void Clear()
    {
        nameSO = null;
    }

    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override bool Check()
    {
        GameObject go = GameObject.Find(nameSO.value);
        return (go && go.activeInHierarchy);
    }
}
