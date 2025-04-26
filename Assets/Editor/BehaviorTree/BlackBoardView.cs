using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class BlackBoardView
{
    private BTBlackboard blackboard;
    private VisualElement root;
    private TextField valueNameText;
    private DropdownField valueTypeDrop;
    private Button addValueBtn;
    private Button delValBtn;
    private ListView valuesListView;
    private VisualTreeAsset variableViewTree;

    private List<Type> valueTypes = new List<Type>(); 

    public BlackBoardView(VisualElement root, BTBlackboard blackboard)
    {
        this.root = root;
        this.blackboard = blackboard;
        this.variableViewTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/BehaviorTree/ValueListItem.uxml");
        valueNameText = root.Q<TextField>("valName");
        addValueBtn = root.Q<Button>("addBtn");
        delValBtn = root.Q<Button>("delBtn");
        valuesListView = root.Q<ListView>("ValueList");
        valueTypeDrop = root.Q<DropdownField>("valType");

        
        addValueBtn.RegisterCallback<ClickEvent>(callback => { AddValue(); });
        delValBtn.RegisterCallback<ClickEvent>(callback => { DeleteValue(); });
        valueTypeDrop.choices = new List<string>();
        FlashView();
    }
    
    private void AddValue()
    {
        blackboard.AddValue(valueNameText.text, valueTypes[valueTypeDrop.index]);
        valuesListView.Rebuild();
    }

    private void DeleteValue()
    {
        if (!blackboard) return;
        if (valuesListView.selectedItem == null) return;
        string key = (valuesListView.selectedItem as BTBlackBoardValSOBase).key;
        blackboard.DeleteValue(key);
        valuesListView.itemsSource = blackboard.values;
        valuesListView.Rebuild();
    }

    public void FlashView()
    {
        if (!blackboard) return;
        UpdateValueTypeSelector();
        GenerateValuesListView();
        valuesListView.Rebuild();
    }

    private void GenerateValuesListView()
    {
        valuesListView.Clear();
        valuesListView.makeItem = () =>
        {
            TemplateContainer variableViewInstance = variableViewTree.CloneTree();
            return variableViewInstance;
        };
        valuesListView.bindItem = (item, index) =>
        {
            item.Q<Label>("ValueName").text = blackboard.values[index].key;
            var value = blackboard.values[index];
            SerializedObject serializedObject = new SerializedObject(value);
            SerializedProperty property = serializedObject.FindProperty("value");
            PropertyField propertyField = item.Q<PropertyField>();
            propertyField.label = "";
            propertyField.BindProperty(property);
            propertyField.Bind(serializedObject);
            propertyField.RegisterValueChangeCallback((evt)=>
            {
                OnValueChangedCallback(evt, value.key);
            });
        };
        valuesListView.itemsSource = blackboard.values;
    }

    private void OnValueChangedCallback(SerializedPropertyChangeEvent evt,string key)
    {
        blackboard.SaveToAsset(key);
    }

    private void UpdateValueTypeSelector()
    {
        valueTypes.Clear();
        valueTypeDrop.choices.Clear();
        BlackboardTools.GetAllChildValueType().ForEach(type =>
        {
            valueTypes.Add(type);
            valueTypeDrop.choices.Add(type.Name);
        });
    }
}
