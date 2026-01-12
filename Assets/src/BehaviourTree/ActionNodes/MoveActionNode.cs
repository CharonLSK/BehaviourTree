
using System;
using TMPro.EditorUtilities;
using UnityEngine;

public class MoveToPosNode: ActionBtNode
{
    [BT_BBAttr(BT_BbDataBase.BTValType.tFloat)]
    public string speedValName;
    private BtFloatVal speedVal;

    [BT_BBAttr(BT_BbDataBase.BTValType.tVector3)]
    public string posValName;
    private BtVector3Val posVal;

    private Vector3 moveSpace;
    private float toleranceSqr = 0.1f * 0.1f;
    public override void Active(GameObject obj)
    {
        base.Active(obj);
        speedVal = btBlackboard.GetValue<BtFloatVal>(speedValName);
    }


    private bool checkArrive()
    {
        Vector3 offset = posVal.value - belongGameobj.transform.position;
        return offset.sqrMagnitude < toleranceSqr;
    }

    private void MoveToDir()
    {
        belongGameobj.transform.position += moveSpace * Time.deltaTime;
    }

    protected override void OnStart()
    {
        posVal = btBlackboard.GetValue<BtVector3Val>(posValName);
        moveSpace = (posVal.value - belongGameobj.transform.position).normalized;
        moveSpace *= speedVal.value;
    }

    protected override BTNodeState OnUpdate()
    {
        if (checkArrive())
            return BTNodeState.Success;
        MoveToDir();
        return BTNodeState.Running;
    }

    protected override void OnStop()
    {
        posVal = null;
    }
}

