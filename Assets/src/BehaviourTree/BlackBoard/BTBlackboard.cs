using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif


public class BTBlackboard : ScriptableObject
{
    public Dictionary<string, BTBlackBoardValSOBase> valuesDic;
    public List<BTBlackBoardValSOBase> values = new List<BTBlackBoardValSOBase>();

    
    public T GetValue<T>(string key) where T:BTBlackBoardValSOBase
    {
        if (valuesDic == null) FlashValueDic();
        if (valuesDic.TryGetValue(key, out var v))
            return v as T;
        return null;
    }

    public bool SetValue<U>(string key, U value)
    {
        var val = GetValue<BTBlackBoardValSO<U>>(key);
        if (val == null) return false;
        val.value = value;
#if UNITY_EDITOR
        EditorUtility.SetDirty(val);
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
#endif
        return true;
    }

    public void AddValue(string key, Type valueType)
    {
        if (string.IsNullOrEmpty(key) || key.Equals("None"))
        {
            return;
        }
        string newKey = new string(
            key.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray()
        );
        if (valuesDic == null) FlashValueDic();
        if (valuesDic.ContainsKey(key))
        {
            Debug.LogWarning($"value {key} is already exists");
            return;
        }

        var value = (BTBlackBoardValSOBase) ScriptableObject.CreateInstance(valueType);
        value.name = "Value_" + valueType.ToString() + "_" + newKey ;
        value.key = newKey;
        values.Add(value);
        valuesDic.Add(newKey, value);

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            AssetDatabase.AddObjectToAsset(value, this);
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
#endif
    }
    
#if UNITY_EDITOR
    public void SaveToAsset(string key)
    {
        if (Application.isPlaying) return;
        if (valuesDic == null) FlashValueDic();
        if (valuesDic.TryGetValue(key, out var val))
        {
            EditorUtility.SetDirty(val);
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
#endif
    public void DeleteValue(string key)
    {
        Debug.Log(" run delete value ,key is " + key);
         BTBlackBoardValSOBase val;
         if (valuesDic == null) FlashValueDic();
         valuesDic.TryGetValue(key, out val);
        if (val)
        {
            values.Remove(val);
            valuesDic.Remove(key);
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                AssetDatabase.RemoveObjectFromAsset(val);
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
            }
#endif
        }
    }

    public BTBlackboard Clone()
    {
        BTBlackboard bb = Instantiate(this);
        bb.values = values.ConvertAll(so => so.Clone());
        return bb;
    }
    
    public void FlashValueDic()
    {
        if (valuesDic == null)
            valuesDic = new Dictionary<string, BTBlackBoardValSOBase>();
        else 
            valuesDic.Clear();
        values.ForEach(n => { valuesDic.Add(n.key, n);});
    }
}