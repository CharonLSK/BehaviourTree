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
        BTBlackboardEditorUtility.AddValue(blackboard, valueNameText.text, valueTypes[valueTypeDrop.index]);
        valuesListView.Rebuild();
    }

    private void DeleteValue()
    {
        if (!blackboard) return;
        if (valuesListView.selectedItem == null) return;
        string key = (valuesListView.selectedItem as BT_BbDataBase).key;
        BTBlackboardEditorUtility.DeleteValue(blackboard, key);
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
            var dataBase = blackboard.GetValueByIdx(index);
            item.Q<Label>("ValueName").text = String.Format("{0}_({1})", dataBase.key, dataBase.valType.ToString());
            VisualElement fieldElement = null;
            void dataChangeFailure(BT_BbDataBase dataBase)
            {
                Debug.LogWarning($"类型转换错误，请检查类型设置。对象名称： {dataBase.key},  设置类型：{dataBase.valType.ToString()} ");
            }
            switch (dataBase.valType)
            {
                case BT_BbDataBase.BTValType.tInt:
                    BtIntVal iData = dataBase as BtIntVal;
                    IntegerField intFiled = new IntegerField();
                    fieldElement  = intFiled;
                    if (iData == null)
                    {
                        dataChangeFailure(dataBase);
                        break;
                    }
                    intFiled.value = iData.value;
                    intFiled.RegisterValueChangedCallback(evt =>
                    {
                        iData.value = evt.newValue;
                        OnValueChangedCallback();
                    });
                    break;
                case BT_BbDataBase.BTValType.tFloat:
                    BtFloatVal fData = dataBase as BtFloatVal;
                    FloatField floatFiled = new FloatField();
                    fieldElement  = floatFiled;
                    if (fData == null)
                    {
                        dataChangeFailure(dataBase);
                        break;
                    }
                    floatFiled.value = fData.value;
                    floatFiled.RegisterValueChangedCallback(evt =>
                    {
                        fData.value = evt.newValue;
                        OnValueChangedCallback();
                    });
                    break;
                case BT_BbDataBase.BTValType.tBool:
                    BtBoolVal bData = dataBase as BtBoolVal;
                    if (bData == null)
                    {
                        dataChangeFailure(dataBase);
                        break;
                    }
                    var toggle = new Toggle();
                    toggle.value = bData.value;
                    toggle.RegisterValueChangedCallback(evt =>
                    {
                        bData.value = evt.newValue;
                        OnValueChangedCallback();
                    });
                    fieldElement  = toggle;
                    break;
                case BT_BbDataBase.BTValType.tString:
                    BtStringVal strData = dataBase as BtStringVal;
                    if (strData == null)
                    {
                        dataChangeFailure(dataBase);
                        break;
                    }
                    var textFeild = new TextField();
                    textFeild.value = strData.value;
                    textFeild.RegisterValueChangedCallback(evt =>
                    {
                        strData.value = evt.newValue;
                        OnValueChangedCallback();
                    });
                    fieldElement  = textFeild;
                    break;
                case BT_BbDataBase.BTValType.tTrans:
                    BtTransformVal transData = dataBase as BtTransformVal;
                    if (transData == null)
                    {
                        dataChangeFailure(dataBase);
                        break;
                    }
                    var transField = new ObjectField();
                    transField.objectType = typeof(Transform);
                    transField.allowSceneObjects = true;
                    transField.value = transData.value;
                    transField.RegisterValueChangedCallback(evt =>
                    {
                        transData.value = evt.newValue as Transform;
                        OnValueChangedCallback();
                    });
                    fieldElement  = transField;
                    break;
                case BT_BbDataBase.BTValType.tVector3:
                    BtVector3Val v3Data = dataBase as BtVector3Val;
                    if (v3Data == null)
                    {
                        dataChangeFailure(dataBase);
                        break;
                    }
                    Vector3Field vector3Field = new Vector3Field();
                    vector3Field.value = v3Data.value;
                    vector3Field.RegisterValueChangedCallback(evt =>
                    {
                        v3Data.value = evt.newValue;
                        OnValueChangedCallback();
                    });
                    fieldElement  = vector3Field;
                    break;
                
            }
            VisualElement fieldRoot = item.Q<VisualElement>("filedRoot");
            if (fieldElement == null)
            {
                fieldElement  = new Label("不支持类型");
            }
            fieldRoot.Add(fieldElement);
        };
        valuesListView.itemsSource = blackboard.Values;
    }

    private void OnValueChangedCallback()
    {
        BTBlackboardEditorUtility.SaveAll(blackboard);
    }

    private void UpdateValueTypeSelector()
    {
        valueTypes.Clear();
        valueTypeDrop.choices.Clear();
        BTBlackboardEditorUtility.GetAllChildValueType().ForEach(type =>
        {
            valueTypes.Add(type);
            valueTypeDrop.choices.Add(type.Name);
        });
    }
}
