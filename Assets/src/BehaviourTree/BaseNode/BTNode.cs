using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class BTNode : ScriptableObject
{
    public enum  BTNodeState
    {
        Running,
        Failure,
        Success,
    }
    public enum AllowedChildCount
    {
        None = 0,
        Single = 1,
        Multiple = int.MaxValue,
    }
    
    [FormerlySerializedAs("state")] [HideInInspector] public BTNodeState btNodeState = BTNodeState.Running;
    [HideInInspector] public bool started = false;
    public string guid;
    [HideInInspector] public abstract AllowedChildCount ChildCntType {get;}
    
    [HideInInspector] public BTBlackboard btBlackboard;
    public List<BTNode> ChildrensList = new List<BTNode>();
    public List<BTNode> GetChildrensList { get { return ChildrensList; } }
    public bool ChildNodeCntToMax { get { return ChildrensList.Count >= (int) ChildCntType; } }

    protected bool isActive = false;
    protected GameObject belongGameobj;

    
#if UNITY_EDITOR
    [HideInInspector]public Vector2 pos; //存储编译器中节点的坐标
    public void SortChildrenByX()
    {
        ChildrensList.Sort((left, right) =>
        {
            return left.pos.x < right.pos.x ? -1 : 1;
        });
    }
#endif

    private void OnEnable()
    {
        isActive = false;
        started = false;
    }

    public virtual void AddChild(BTNode child)
    {
        if (ChildrensList.Count >= (int)ChildCntType) {
            Debug.LogWarning($"{name} 已达最大子节点数 {(int)ChildCntType}");
            return;
        }
        ChildrensList.Add(child);
    }

    public virtual void RemoveChild(BTNode child)
    {
        bool removed = ChildrensList.Remove(child);
        if (!removed)
            Debug.LogWarning($"node {name} dont have child node {child.name}");
    }
    
    public virtual void Active(GameObject obj)
    {
        if (isActive) return;
        belongGameobj = obj;
        isActive = false;
    }

    public virtual void Clear()
    {
        isActive = false;
        belongGameobj = null;
    }

    public BTNodeState Update()
    {
        if (started == false)
        {
            OnStart();
            started = true;
        }
        btNodeState = OnUpdate();
        if (btNodeState == BTNodeState.Failure || btNodeState == BTNodeState.Success)
        {
            OnStop();
            started = false;
        }
        return btNodeState;
    }

    protected abstract void OnStart();
    protected abstract BTNodeState OnUpdate();
    protected abstract void OnStop();

    public virtual BTNode Clone()
    {
        BTNode btNode = Instantiate(this);
        return btNode;
    }

    public void GetFields()
    {
        throw new System.NotImplementedException();
    }
}
