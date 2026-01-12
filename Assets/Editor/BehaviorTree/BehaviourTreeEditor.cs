using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Callbacks;
using UnityEditor.Timeline.Actions;
using Object = UnityEngine.Object;

public class BehaviourTreeEditor : EditorWindow
{
    private BehaviourTreeView treeView;
    private BTEditorInspectorView _btEditorInspectorView;
    private ObjectField treeSelector;
    private BehaviourTree selectTree;
    private BehaviourTree behaviourTree;

    private static BehaviourTreeEditor windowInstance;
    private static void SetTitle(string title)
    {
        if (windowInstance == null)
        {
            windowInstance = GetWindow<BehaviourTreeEditor>();
            windowInstance.titleContent = new GUIContent();
        }
        windowInstance.titleContent.text = title;
    }


    [MenuItem("BehaviourTreeEditor/Editor ...")]
    public static void OpenWindow()
    {
        SetTitle("BehaviourTreeEditor");
    }
    
    //选中行为树是自动打开editorwindow
    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceId, int line)
    {
        if (Selection.activeObject is BehaviourTree)
        {
            OpenWindow();
            BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
            SetTitle("BehaviourTreeEditor_" + Selection.activeObject.name);
            return true;
        }

        return false;
    }

    public void OnEnable()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML， uibuilder创建的模板
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/BehaviorTree/BehaviourTreeEditor.uxml");
        visualTree.CloneTree(root);
        
        //加载样式
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/BehaviorTree/BehaviourTreeEditor.uss");
        root.styleSheets.Add(styleSheet);

        //下面两个是自定义的VisualElement,由于都只有一个可以直接通过类获取，方法相当于transform.find
        treeView = root.Q<BehaviourTreeView>();
        _btEditorInspectorView = root.Q<BTEditorInspectorView>();
        treeView.OnNodeSelected = OnNodeSelectionChange;
        treeView.blackboardVE = root.Q<VisualElement>("blackboard");
        
        treeSelector = root.Q<ObjectField>("treeSelector");
        treeSelector.objectType = typeof(BehaviourTree);
        treeSelector.allowSceneObjects = false;
        treeSelector.RegisterValueChangedCallback(evt =>
        {
            OnTreeSelectorChange(evt);
        });
        //运行时切换为行为树的clone,无法在onenable中添加，unity会在启动运行时重新执行此方法，但顺序在onenable之后
        EditorApplication.playModeStateChanged -= OnPlayModeStateChange;
        EditorApplication.playModeStateChanged += OnPlayModeStateChange;
        OnSelectionChange();
    }


    private void OnTreeSelectorChange(ChangeEvent<UnityEngine.Object> evt)
    {
        BehaviourTree tree = evt.newValue as BehaviourTree;
        ChangeSelectTree(tree);
    }

    //关闭窗口触发
    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChange;
        windowInstance = null;
    }

    private BehaviourTree GetSelectTree()
    {
        BehaviourTree tree = Selection.activeObject as BehaviourTree;
        if (!tree)
        {
            if (Selection.activeGameObject)
            {
                BehaviourTreeRunner runner = Selection.activeGameObject.GetComponent<BehaviourTreeRunner>();
                if (runner)
                {
                    tree = runner.tree;
                }
            }
        }
        return tree;
    }

    //unity中的事件，当编辑器选中对象变化时调用
    private void OnSelectionChange()
    {
        BehaviourTree tree = GetSelectTree();
        if (tree)
        {
            if (Application.isEditor)
            {
                if (AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
                    ChangeSelectTree(tree);
            }
            else
                ChangeSelectTree(tree);
        }
    }

    private void ChangeSelectTree(BehaviourTree tree)
    {
        behaviourTree = tree;
        var selectorValue = treeSelector.value;
        if (selectorValue == null || (selectorValue as BehaviourTree) != behaviourTree)
        {
            treeSelector.SetValueWithoutNotify(behaviourTree);
        }
        treeView.FlashView(tree);
        SetTitle("BehaviourTreeEditor_" + tree.name);
    }

    //运行运行模式时切换为clone，editor模式下切换为assets
    private void OnPlayModeStateChange(PlayModeStateChange obj)
    {
        var tree = GetSelectTree();
        switch (obj)
        {
            case PlayModeStateChange.EnteredEditMode:
                if (tree)
                    ChangeSelectTree(tree);
                break;
            case PlayModeStateChange.ExitingEditMode:
                break;
            case PlayModeStateChange.EnteredPlayMode:
                if (tree)
                    ChangeSelectTree(tree);
                break;
            case PlayModeStateChange.ExitingPlayMode:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(obj), obj, null);
        }
    }
    
    //节点选中时更新Inspector面板，防止下层Node持有Inspector面板使用事件传输
    void OnNodeSelectionChange(NodeView node)
    {
        _btEditorInspectorView.UpdateSelection(node);
    }

    private void OnInspectorUpdate()
    {
        treeView?.UpdateNodeStates();
    }
}