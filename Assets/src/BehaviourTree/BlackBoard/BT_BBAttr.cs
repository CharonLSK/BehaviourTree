using System;
/// <summary>
/// 需要通过string的去黑板中获取值
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class BT_BBAttr : Attribute
{
    public BT_BBAttr(BT_BbDataBase.BTValType type)
    {
        tarType = type;
    }
    // 也可以定义可读写的属性
    public BT_BbDataBase.BTValType tarType { get; }
}

/// <summary>
/// 需要显示在inspector面板
/// </summary>
public class BT_BB_DirBind : Attribute
{
}
