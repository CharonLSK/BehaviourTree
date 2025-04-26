using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class BTNode : ScriptableObject
{
    public enum  State
    {
        Running,
        Failure,
        Success,
    }

    [HideInInspector] public State state = State.Running;
    [HideInInspector] public bool started = false;
    [HideInInspector] public string guid;
    public BTBlackboard btBlackboard; 
    
#if UNITY_EDITOR
    [HideInInspector]public Vector2 pos; //存储编译器中节点的坐标

    public void SortChildren()
    {
        //暂时只有com节点有多个子节点
        CompositeBtNode comp = this as CompositeBtNode;
        if (comp)
        {
            comp.children.Sort((left, right) =>
            {
                return left.pos.x < right.pos.x ? -1 : 1;
            });
        }
    }
#endif

    public State Update()
    {
        if (started == false)
        {
            OnStart();
            started = true;
        }

        state = OnUpdate();

        if (state == State.Failure || state == State.Success)
        {
            OnStop();
            started = false;
        }

        return state;
    }

    protected abstract void OnStart();
    protected abstract State OnUpdate();
    protected abstract void OnStop();

    public virtual BTNode Clone()
    {
        BTNode btNode = Instantiate(this);
        return btNode;
    }
}
