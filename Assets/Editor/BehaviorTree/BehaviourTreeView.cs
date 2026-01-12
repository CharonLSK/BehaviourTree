using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class BehaviourTreeView : GraphView
{
    public Action<NodeView> OnNodeSelected;
    private BehaviourTree behaviourTree;
    public VisualElement blackboardVE;
    private BlackBoardView blackboardView;

    //自定义的 VisualElement需要添加，否则不会再Builder的库中出现
    public new class UxmlFactory : UxmlFactory<BehaviourTreeView, GraphView.UxmlTraits>  { }

    public BehaviourTreeView()
    {
        //添加北京并放到最底层
        Insert(0, new GridBackground());
        
        //添加缩放 平移 拖拽 框选
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        
        //获取自定义的样式，添加到当前界面
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/BehaviorTree/BehaviourTreeEditor.uss");
        styleSheets.Add(styleSheet);
        Undo.undoRedoPerformed += OnUndoRedo;
        
    }

    private void OnUndoRedo()
    {
        FlashView(behaviourTree);
        AssetDatabase.SaveAssets();
    }

    NodeView FindNodeView(BTNode btNode)
    {
        return GetNodeByGuid(btNode.guid) as NodeView;
    }

    internal void FlashView(BehaviourTree tree)
    {
        this.behaviourTree = tree;
        graphViewChanged -= onGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += onGraphViewChanged;
        if (tree.rootBtNode == null)
        {
            tree.rootBtNode = tree.CreateNode(typeof(RootBtNode)) as RootBtNode;
            //存储ScriptableObject到assets
            EditorUtility.SetDirty(tree);
            AssetDatabase.SaveAssets();
        }

        
        //创建node
        tree.allNode.ForEach(n => CreateNodeView(n));
        //创建 edge
        tree.allNode.ForEach(n =>
        {
            var children = tree.GetChildren(n);
            NodeView parentView = FindNodeView(n);
            children.ForEach(c =>
            {
                NodeView childView = FindNodeView(c);
                Edge edge = parentView.outputPort.ConnectTo(childView.inputPort);
                AddElement(edge);
            });
        });
        
        //创建blackboard
        if (tree.btBlackboard == null)
        {
            behaviourTree.CreateBlackboard();
        }
        if (blackboardView == null)
        {
            InitBlackboard();
        }
        blackboardView.FlashView();
        
    }
    void InitBlackboard()
    {
        //blackboard
        blackboardView = new BlackBoardView(blackboardVE, behaviourTree.btBlackboard);
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(endPort => endPort.direction != startPort.direction &&
                                               endPort.node != startPort.node).ToList();
    }

    //handle graph change event
    private GraphViewChange onGraphViewChanged(GraphViewChange graphviewchange)
    {
        int group = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Behaviour Tree Change");
        bool deleteFlag = false;
        List<Object> undoLst = new List<Object>();
        //handle node or edge delete
        if (graphviewchange.elementsToRemove != null)
        {
            graphviewchange.elementsToRemove.ForEach(elem =>
            {
                NodeView nodeView = elem as NodeView;
                if (nodeView != null)
                {
                    if (!deleteFlag)
                    {
                        deleteFlag = true;
                        Undo.RecordObject(behaviourTree, "node delete");
                    }
                    behaviourTree.DeleteNode(nodeView.btNode);
                }
                Edge edge = elem as Edge;
                if (edge != null)
                {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;
                    Undo.RecordObject(parentView.btNode,"edge delete");
                    behaviourTree.RemoveChild(parentView.btNode, childView.btNode);
                }
            });
        }
        if (graphviewchange.edgesToCreate != null)
        {
            graphviewchange.edgesToCreate.ForEach(edge =>
            {
                NodeView parentView = edge.output.node as NodeView;
                NodeView childView = edge.input.node as NodeView;
                Undo.RecordObject(parentView.btNode,"");
                behaviourTree.AddChild(parentView.btNode, childView.btNode);
            });
        }
        //handel node pos move, sort child lst
        if (graphviewchange.movedElements != null)
        {
            behaviourTree.allNode.ForEach(n => { n.SortChildrenByX(); });
        }
        Undo.CollapseUndoOperations(group);
        EditorUtility.SetDirty(behaviourTree);
        return graphviewchange;
    }

    //create mouse btn 1 click menu
    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        {
            evt.menu.AppendAction($"Flash", (a) => { FlashView(behaviourTree);});
        }
        
        Vector2 pos = contentViewContainer.WorldToLocal(evt.mousePosition);
        //action node
        {
            var types = TypeCache.GetTypesDerivedFrom<ActionBtNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}]/{type.Name}", (a) => CreateNode(type, pos));
            }
        }
        //composite
        {
            var types = TypeCache.GetTypesDerivedFrom<CompositeBtNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}]/{type.Name}", (a) => CreateNode(type, pos));
            }
        }
        //decorator
        {
            var types = TypeCache.GetTypesDerivedFrom<DecoratorBtNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}]/{type.Name}", (a) => CreateNode(type, pos));
            }
        }
        //condition
        {
            var types = TypeCache.GetTypesDerivedFrom<ConditionNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}]/{type.Name}", (a) => CreateNode(type, pos));
            }
        }

        //blackboard 
        // {
        //     evt.menu.AppendAction("Create Blackboard", (a) =>
        //     {
        //         behaviourTree.CreateBlackboard();
        //         InitBlackboard();
        //     });
        // }
    }


    void CreateNode(System.Type type, Vector2 pos)
    {
        Undo.RecordObject(behaviourTree, "Behaviour Tree CreateNode");
        EditorUtility.SetDirty(behaviourTree);
        BTNode btNode = behaviourTree.CreateNode(type);
        btNode.pos = pos;
        CreateNodeView(btNode);
    }

    void CreateNodeView(BTNode btNode)
    {
        NodeView nodeView = new NodeView(btNode);
        nodeView.OnNodeSelected = OnNodeSelected;
        AddElement(nodeView);
    }

    public void UpdateNodeStates()
    {
        nodes.ForEach(n =>
        {
            NodeView view = n as NodeView;
            view.UpdateState();
        });
    }
}
