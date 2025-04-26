using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu()]
public class BehaviourTree : ScriptableObject
{ 
    public BTNode rootBtNode;
    public BTNode.State treeState = BTNode.State.Running;
    public List<BTNode> allNode = new List<BTNode>(); 
    public BTBlackboard btBlackboard;


    public BTNode.State Update()
    {
        if (rootBtNode.state == BTNode.State.Running)
        {
            treeState = rootBtNode.Update();
        }
        return treeState;
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
        Debug.Log(type.Name);
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
        DecoratorBtNode decoratorBt = parent as DecoratorBtNode;;
        if (decoratorBt)
        {
            decoratorBt.child = child;
        }
        
        RootBtNode rootBtNode = parent as RootBtNode;;
        if (rootBtNode)
        {
            rootBtNode.child = child;
        }

        CompositeBtNode compositeBt = parent as CompositeBtNode;
        if (compositeBt)
        {
            compositeBt.children.Add(child);
        }
    }

    public void RemoveChild(BTNode parent, BTNode child)
    {
        DecoratorBtNode decoratorBt = parent as DecoratorBtNode;;
        if (decoratorBt)
        {
            decoratorBt.child = null;
        }
        CompositeBtNode compositeBt = parent as CompositeBtNode;
        if (compositeBt)
        {
            compositeBt.children.Remove(child);
        }
        RootBtNode rootBtNode = parent as RootBtNode;;
        if (rootBtNode)
        {
            rootBtNode.child = null;
        }
    }
    
    public List<BTNode> GetChildren(BTNode parent)
    {
        List<BTNode> lst = new List<BTNode>();
        DecoratorBtNode decoratorBt = parent as DecoratorBtNode;
        if (decoratorBt && decoratorBt.child != null)
        {
            lst.Add(decoratorBt.child);
        }
        CompositeBtNode compositeBt = parent as CompositeBtNode;
        if (compositeBt && compositeBt.children != null)
        {
            return compositeBt.children;
        }
        RootBtNode rootBtNode = parent as RootBtNode;;
        if (rootBtNode && rootBtNode.child != null)
        {
            lst.Add(rootBtNode.child);
        }

        return lst;
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
            tree.allNode.ForEach(n => { n.btBlackboard = btBlackboard;});
        }
        return tree;
    }


}
