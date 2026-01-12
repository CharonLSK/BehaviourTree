using UnityEngine;
using UnityEngine.Serialization;

public class CheckDistance : ConditionNode
{
    private Transform selfTrans;

    [BT_BBAttr(BT_BbDataBase.BTValType.tFloat)]
    public string SightValName;
    private BtFloatVal sightSO;
    private float sightSqr;

    [BT_BBAttr(BT_BbDataBase.BTValType.tString)]
    public string stringValName;
    private BtStringVal tarName;
    
    public override void Active(GameObject obj)
    {
        base.Active(obj);
        selfTrans = belongGameobj.transform;
        sightSO = btBlackboard.GetValue<BtFloatVal>(SightValName);
        tarName = btBlackboard.GetValue<BtStringVal>(stringValName);
    }

    protected override void OnStart()
    {
        sightSqr = sightSO.value * sightSO.value;
    }

    protected override void OnStop()
    {
    }

    protected override bool Check()
    {
        GameObject target = GameObject.Find(tarName.value) as GameObject;
        return (target.transform.position - belongGameobj.transform.position).sqrMagnitude < sightSqr;
    }
}
