using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;using UnityEditor.Experimental.GraphView;
using UnityEngine;

public abstract class BTBlackBoardValSOBase : ScriptableObject
{
    public string key;

    public virtual BTBlackBoardValSOBase Clone()
    {
        return Instantiate(this);
    }
}

public abstract class BTBlackBoardValSO<T> : BTBlackBoardValSOBase
{
    public T value = default;
    public void Init(string key, T defaultValue)
    {
        // 只有在运行时实例化后再调用这个方法来赋值
        this.key = key;
        this.value = defaultValue;
    }
}

public static class BlackboardTools
{
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

            Type baseType = typeof(BTBlackBoardValSOBase);
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
