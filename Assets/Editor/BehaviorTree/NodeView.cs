using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using PlasticGui.Help;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;
public class NodeView : UnityEditor.Experimental.GraphView.Node
{
    public Action<NodeView> OnNodeSelected;
    public BTNode btNode;
    public Port inputPort;
    public Port outputPort;
    private Label des;
    private VisualElement selectionBorder;
    public NodeView(BTNode btNode) : base("Assets/Editor/BehaviorTree/NodeView.uxml")
    {
        des = this.Q<Label>("description");
        selectionBorder = this.Q<VisualElement>("selection-borders");
        this.btNode = btNode;
        this.title = btNode.name;
        this.viewDataKey = btNode.guid;
        des.text = btNode.GetType().ToString();
        //节点在编译器中的坐标
        style.left = btNode.pos.x;
        style.top = btNode.pos.y;

        CreateInputPorts();
        CreateOunputPorts();
        SetupClasses();
    }

    private void SetupClasses()
    {
        if (btNode is ActionBtNode)
        {
            AddToClassList("Action");
        }
        else if (btNode is CompositeBtNode)
        {
            AddToClassList("Composite");
        }
        else if (btNode is DecoratorBtNode)
        {
            AddToClassList("Decorator");
        }
        else if (btNode is RootBtNode)
        {
            AddToClassList("Root");
        }
        else if (btNode is ConditionNode)
        {
            AddToClassList("Condition");
        }
    }
    private void CreateInputPorts()
    {
        if (btNode is ActionBtNode)
        {
            inputPort = InstantiatePort(Orientation.Vertical, Direction.Input,
                Port.Capacity.Single, typeof(bool));
        }
        else if (btNode is CompositeBtNode)
        {
            inputPort = InstantiatePort(Orientation.Vertical, Direction.Input,
                Port.Capacity.Single, typeof(bool));
        }
        else if (btNode is DecoratorBtNode)
        {
            inputPort = InstantiatePort(Orientation.Vertical, Direction.Input,
                Port.Capacity.Single, typeof(bool));
        }
        else if (btNode is RootBtNode)
        {
        }
        else if (btNode is ConditionNode)
        {
            inputPort = InstantiatePort(Orientation.Vertical, Direction.Input,
                Port.Capacity.Single, typeof(bool));
        }
        if (inputPort != null)
        {
            ResizePort(inputPort);
            inputPort.style.flexDirection = FlexDirection.Column;
            inputContainer.Add(inputPort);
        }
    }

    private void CreateOunputPorts()
    {
        //叶子节点，不需要outport
        if (btNode is ActionBtNode)
        {
        }
        else if (btNode is CompositeBtNode)
        {
            //最后一个参数是port承载的数据类型，在连线时需要输入输出类型匹配,没什么用随便给一个
            outputPort = InstantiatePort(Orientation.Vertical, Direction.Output,
                Port.Capacity.Multi, typeof(bool));
        }
        else if (btNode is DecoratorBtNode)
        {
            outputPort = InstantiatePort(Orientation.Vertical, Direction.Output,
                Port.Capacity.Single, typeof(bool));
        }
        else if (btNode is RootBtNode)
        {
            outputPort = InstantiatePort(Orientation.Vertical, Direction.Output,
                Port.Capacity.Single, typeof(bool));
        }
        
        if (outputPort != null)
        {
            ResizePort(outputPort);
            outputPort.style.flexDirection = FlexDirection.ColumnReverse;
            outputContainer.Add(outputPort);
        }

    }

    private void ResizePort(Port port)
    {
        //port里除了节点的图标还带有一个lable用不到调小并放大port方便操作
        port.name = "";
        var label = port.Q<Label>("type");
        if (label != null)
        {
            label.text = "";
            label.style.width = 0;
            label.style.height = 0;
        }
        var connector = port.Q<VisualElement>("connector");
        var cap       = connector.Q<VisualElement>("cap");
        //跳大Port大小，方便点击
        connector.style.width  = 14;
        connector.style.height = 14;
        cap.style.width  = 6;
        cap.style.height = 6;
    }


    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Undo.RecordObject(btNode, "Behaviour Tree Set Position");
        EditorUtility.SetDirty(btNode); 
        btNode.pos.x = newPos.xMin;
        btNode.pos.y = newPos.yMin;
    }

    public override void OnSelected()
    {
        base.OnSelected();
        if (OnNodeSelected != null)
        {
            OnNodeSelected.Invoke(this);
        }
    }

    //显示当前运行状态
    public void UpdateState()
    {
        if (!Application.isPlaying) return;
        RemoveFromClassList("Failure");
        RemoveFromClassList("Success");
        RemoveFromClassList("Running");

        switch (btNode.btNodeState)
        {
            case BTNode.BTNodeState.Running:
                if (btNode.started) 
                    AddToClassList("Running");  
                break;
            case BTNode.BTNodeState.Failure:
                AddToClassList("Failure");
                break;
            case BTNode.BTNodeState.Success:
                AddToClassList("Success");
                break;
        }
    }
}
