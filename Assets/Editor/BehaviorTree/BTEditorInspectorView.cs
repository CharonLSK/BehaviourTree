using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

public class BTEditorInspectorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<BTEditorInspectorView, VisualElement.UxmlTraits> { }

    private List<VisualElement> propertyLst;
    private UnityEditor.Editor editor;
    
    public BTEditorInspectorView()
    {
        propertyLst = new List<VisualElement>();
    }


    public void UpdateSelection(NodeView nodeView)
    {
        Clear();
        UnityEngine.Object.DestroyImmediate(editor);
        editor = UnityEditor.Editor.CreateEditor(nodeView.btNode);
        IMGUIContainer container = new IMGUIContainer(() =>
        {
            if (editor.target)
            {
                editor.OnInspectorGUI();
            }
        });
        Add(container);
    }


    public void UpdateSelection1(NodeView nodeView)
    {
        var type = nodeView.btNode.GetType();
        var members = type
            .GetMembers(BindingFlags.Public | BindingFlags.Instance)
            .Where(m =>
                // 只保留字段或属性
                (m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property)
                // 并且标记了 BT_BBAttr 特性
                && m.IsDefined(typeof(BT_BBAttr), inherit: false)
            );
        
        foreach (var member in members)
        {
            
            
            //获取特性
            var attr = member.GetCustomAttributes<BT_BBAttr>(inherit: false);
            
            //获取当前值
            string value = null;
            if (member is FieldInfo fi)
            {
                value = fi.GetValue(nodeView.btNode) as string;
            }
            else if (member is PropertyInfo pi)
            {
                value = pi.GetValue(nodeView.btNode) as string;
            }
        }
    }


    private void AddPropertyFile(SerializedObject so)
    {
        var pf = new PropertyField(so.FindProperty("someField"), "Some Field");
        this.Add(pf);
        this.Bind(so);
    }
}
