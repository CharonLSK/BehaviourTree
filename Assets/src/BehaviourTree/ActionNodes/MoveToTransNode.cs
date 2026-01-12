using System;
using TMPro.EditorUtilities;
using UnityEngine;

public class MoveToTransNode: ActionBtNode
{
    public string transValName;
    private BtTransformVal tranSO;

    public string speedValName;
    private BtFloatVal speedSO;

    private Vector3 moveSpace;
    private float toleranceSqr = 0.1f * 0.1f;

    public override void Active(GameObject obj)
    {
        base.Active(obj);
        speedSO = btBlackboard.GetValue<BtFloatVal>(speedValName);
        tranSO = btBlackboard.GetValue<BtTransformVal>(transValName);
    }

    private bool checkArrive()
    {
        Vector3 offset = tranSO.value.position - belongGameobj.transform.position;
        return offset.sqrMagnitude < toleranceSqr;
    }

    private void UpdateMoveSpace()
    {
        moveSpace = (tranSO.value.position - belongGameobj.transform.position)
            .normalized * speedSO.value;
    }

    protected override void OnStart()
    {
        tranSO = btBlackboard.GetValue<BtTransformVal>(transValName);
        belongGameobj.transform.LookAt(tranSO.value);
    }

    protected override BTNodeState OnUpdate()
    {
        if (checkArrive())
        {
            return BTNodeState.Success;
        }
        UpdateMoveSpace();

        belongGameobj.transform.position += moveSpace * Time.deltaTime;
        return BTNodeState.Running;
    }

    protected override void OnStop()
    {
    }
}