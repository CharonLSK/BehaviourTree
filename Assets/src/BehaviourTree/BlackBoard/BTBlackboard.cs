using System;
using System.Collections.Generic;
using UnityEngine;

public class BTBlackboard : ScriptableObject
{
    [SerializeReference] private List<BT_BbDataBase> values;
    private Dictionary<string, BT_BbDataBase> valuesDic;

    /// <summary>只读访问黑板条目列表</summary>
    public List<BT_BbDataBase> Values => values;

    private void OnEnable()
    {
        if (values == null)
        {
            values = new List<BT_BbDataBase>();
        }
        RebuildDic();
    }

    public T GetValue<T>(string key) where T : BT_BbDataBase
    {
        if (valuesDic == null)
            RebuildDic();
        valuesDic.TryGetValue(key, out var entry);
        return entry as T;
    }

    public bool SetValue<U>(string key, U newValue)
    {
        var entry = GetValue<BT_BbData<U>>(key);
        if (entry == null) return false;
        entry.value = newValue;
        return true;
    }

    public BT_BbDataBase GetValueByIdx(int idx)
    {
        if (idx < 0 || idx >= values.Count)
            return null;
        return values[idx];
    }

    public BTBlackboard Clone()
    {
        var clone = Instantiate(this);
        clone.values = values.ConvertAll(v => v.Clone());
        clone.RebuildDic();
        return clone;
    }

    public void RebuildDic()
    {
        if (valuesDic == null)
            valuesDic = new Dictionary<string, BT_BbDataBase>();
        else
            valuesDic.Clear();

        foreach (var entry in values)
        {
            if (!string.IsNullOrEmpty(entry.key))
                valuesDic[entry.key] = entry;
        }
    }
}
