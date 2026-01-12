using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;using UnityEditor.Experimental.GraphView;
using UnityEngine;

[Serializable]
public abstract class BT_BbDataBase
{
    public enum BTValCompType
    {
        Bigger,
        Lower
    }
    public enum BTValType
    {
        tInt = 1,
        tBool = 2,
        tFloat = 3,
        tTrans = 4,
        tVector3 = 5,
        tString = 6,
    }
    
    public string key;
    protected BTValCompType compType = BTValCompType.Lower;
    public virtual BTValType valType { get; }

    public BT_BbDataBase()  {}
    public BT_BbDataBase(BT_BbDataBase tar)
    {
        this.key = tar.key;
        this.compType = tar.compType;
    }
    public abstract BT_BbDataBase Clone();
}

[Serializable]
public abstract class BT_BbData<T> : BT_BbDataBase
{

    public T value = default(T);

    public void Init(string key, T defaultValue)
    {
        // 只有在运行时实例化后再调用这个方法来赋值
        this.key = key;
        this.value = defaultValue;
    }
    public BT_BbData():base()
    {}
    public BT_BbData(BT_BbData<T> tar) : base(tar)        
    {
        this.value = tar.value;
    }
}

[Serializable]
public class BtBoolVal : BT_BbData<bool>
{
    public BtBoolVal() : base()
    {
    }
    public BtBoolVal(BtBoolVal tar) : base(tar)
    {
        
    }

    public override BTValType valType
    {
        get { return BTValType.tBool; }
    }

    public override BT_BbDataBase Clone()
    {
        return new BtBoolVal(this);
    }
}
[Serializable]
public class BtFloatVal : BT_BbData<float>
{
    public BtFloatVal() : base()
    {
    }
    public BtFloatVal(BtFloatVal tar) : base(tar)
    {
        
    }
    public override BTValType valType
    {
        get { return BTValType.tFloat; }
    }

    public override BT_BbDataBase Clone()
    {
        return new BtFloatVal(this);
    }


}
[Serializable]
public class BtIntVal: BT_BbData<int>
{
    public BtIntVal() : base()
    {
    }
    public BtIntVal(BtIntVal tar) : base(tar)
    {
        
    }
    public override BTValType valType
    {
        get { return BTValType.tInt; }
    }

    public override BT_BbDataBase Clone()
    {
        return new BtIntVal(this);
    }


}

[Serializable]
public class BtTransformVal : BT_BbData<Transform>
{
    public BtTransformVal() : base()
    {
    }
    public BtTransformVal(BtTransformVal tar) : base(tar)
    {
        
    }
    public override BTValType valType
    {
        get { return BTValType.tTrans; }
    }

    public override BT_BbDataBase Clone()
    {
        return new BtTransformVal(this);
    }
}

[Serializable]
public class BtVector3Val : BT_BbData<Vector3>
{
    public BtVector3Val() : base()
    {
    }
    public BtVector3Val(BtVector3Val tar) : base(tar)
    {
        
    }
    public override BTValType valType
    {
        get { return BTValType.tVector3; }
    }
    public override BT_BbDataBase Clone()
    {
        return new BtVector3Val(this);
    }
}

[Serializable]
public class BtStringVal : BT_BbData<string>
{
    public BtStringVal() : base()
    {
    }
    public BtStringVal(BtStringVal tar) : base(tar)
    {
        
    }
    
    public override BTValType valType
    {
        get { return BTValType.tString; }
    }

    public override BT_BbDataBase Clone()
    {
        return new BtStringVal(this);
    }
}


#if UNITY_EDITOR

#endif
