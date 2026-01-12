using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public static class BTBlackboardEditorUtility
{
    /// <summary>
    /// 向黑板中新增一个值，并将子 SO 添加到主资产中
    /// </summary>
    /// <param name="blackboard">目标黑板 ScriptableObject</param>
    /// <param name="key">黑板键名</param>
    /// <param name="valueType">黑板值的类型</param>
    public static void AddValue(BTBlackboard blackboard, string key, Type valueType)
    {
        if (Application.isPlaying) return;
        if (string.IsNullOrWhiteSpace(key))
        {
            Debug.LogWarning("Key不能为空");
            return;
        }
        key = key.Replace(" ", "");
        blackboard.RebuildDic();
        if (blackboard.GetValue<BT_BbDataBase>(key) != null)
        {
            Debug.LogWarning($"黑板中已存在此key: '{key}'");
            return;
        }
        var val = Activator.CreateInstance(valueType) as BT_BbDataBase;
        val.key = key;
        if (val == null)
        {
            Debug.LogWarning($"添加类型错误，类型: {valueType.Name}");
        }
        else
        {
            // 更新列表与字典
            blackboard.Values.Add(val);
            blackboard.RebuildDic();
            EditorUtility.SetDirty(blackboard);
            AssetDatabase.SaveAssets();
        }
    }
    
    public static void SaveToAsset(BTBlackboard blackboard, string key)
    {
        if (Application.isPlaying) return;
        EditorUtility.SetDirty(blackboard);
        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// 从黑板中删除指定键的值，并移除子资产
    /// </summary>
    public static void DeleteValue(BTBlackboard blackboard, string key)
    {
        if (Application.isPlaying) return;
        blackboard.RebuildDic();
        var entry = blackboard.GetValue<BT_BbDataBase>(key);
        if (entry == null)
        {
            Debug.LogWarning($"未找到键 '{key}'，无法删除");
            return;
        }
        // 从列表中移除
        blackboard.Values.Remove(entry);
        blackboard.RebuildDic();
        SaveAll(blackboard);

    }

    /// <summary>
    /// 保存黑板相关资产
    /// </summary>
    public static void SaveAll(BTBlackboard blackboard)
    {
        if (Application.isPlaying) return;
        EditorUtility.SetDirty(blackboard);
        AssetDatabase.SaveAssets();
    }
    
    public static List<Type> GetAllChildValueType()
    {
        List<Type> allChildType = new List<Type>();
        
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types.Where(t => t != null).ToArray();
            }

            Type baseType = typeof(BT_BbDataBase);
            foreach (var type in types)
            {
                if (type != baseType && baseType.IsAssignableFrom(type) && !type.IsAbstract)
                {
                    allChildType.Add(type);
                }
            }
        }
        return allChildType;
    }

    

}
