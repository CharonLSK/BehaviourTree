using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Serialization;

[CreateAssetMenu()]
public class BehaviourTree : ScriptableObject
{ 
    public BTNode rootBtNode;
    public BTNode.BTNodeState treeBtNodeState = BTNode.BTNodeState.Running;
    public List<BTNode> allNode = new List<BTNode>(); 
    public BTBlackboard btBlackboard;
    
    public GameObject tarGameObj;
    private bool isActive = false;
    

    //TODO: blackboard data init from json
    public void Active(GameObject obj)
    {
        if (isActive) return;
        isActive = true;
        tarGameObj = obj;
        allNode.ForEach(n => {n.Active(obj);});
    }
    
    // public void Clear()
    // {
    //     isActive = false;
    //     tarGameObj = null;
    //     allNode.ForEach(n => {n.Clear();});
    //
    // }

    public BTNode.BTNodeState Update()
    {
        if (rootBtNode.btNodeState == BTNode.BTNodeState.Running)
        {
            treeBtNodeState = rootBtNode.Update();
        }
        return treeBtNodeState;
    }

    public void CreateBlackboard()
    {
        if (btBlackboard) return;
        btBlackboard = ScriptableObject.CreateInstance<BTBlackboard>();
        btBlackboard.name = "Blackboard";
        if (!Application.isPlaying)
        {
            AssetDatabase.AddObjectToAsset(btBlackboard, this);
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
        allNode.ForEach(n => n.btBlackboard = btBlackboard);
    }

    public void DeleteBlackBoard()
    {
        if(!btBlackboard) return;
        AssetDatabase.RemoveObjectFromAsset(btBlackboard);
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        btBlackboard = null;
        allNode.ForEach(n => n.btBlackboard = null);
    }

    public BTNode CreateNode(System.Type type)
    {
        BTNode btNode = ScriptableObject.CreateInstance(type) as BTNode;
        btNode.name = type.Name;
        btNode.guid = GUID.Generate().ToString();
        allNode.Add(btNode);
        if (!Application.isPlaying)
        {
            AssetDatabase.AddObjectToAsset(btNode, this);
            AssetDatabase.SaveAssets();
        }
        return btNode;
    }

    public void DeleteNode(BTNode btNode)
    {
        allNode.Remove(btNode);
        AssetDatabase.RemoveObjectFromAsset(btNode);
        AssetDatabase.SaveAssets();
    }


    public void AddChild(BTNode parent, BTNode child)
    {
        if (parent.ChildNodeCntToMax)
        {
            Debug.LogWarning($"Node {parent.name} child cnt ");
            return;
        }

        parent.AddChild(child);
    }

    public void RemoveChild(BTNode parent, BTNode child)
    {
        parent.RemoveChild(child);
    }
    
    public List<BTNode> GetChildren(BTNode parent)
    {

        return parent.GetChildrensList;
    }

    public void Traverse(BTNode node, System.Action<BTNode> visiter)
    {
        if (node)
        {
            visiter.Invoke(node);
            var children = GetChildren(node);
            children.ForEach(n => { Traverse(n, visiter);});
        }
    }

    public BehaviourTree Clone()
    {
        BehaviourTree tree = Instantiate(this);
        tree.rootBtNode = tree.rootBtNode.Clone();
        tree.allNode = new List<BTNode>();
        Traverse(tree.rootBtNode, n => { tree.allNode.Add(n);});
        if (btBlackboard)
        {   
            tree.btBlackboard = tree.btBlackboard.Clone();
            tree.allNode.ForEach(n => { n.btBlackboard = tree.btBlackboard;});
        }
        return tree;
    }


}
