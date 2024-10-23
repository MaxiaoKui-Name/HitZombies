
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;
using SimpleJSON;


namespace cfg
{
public sealed partial class DailyConfig : Luban.BeanBase
{
    public DailyConfig(JSONNode _buf) 
    {
        { if(!_buf["ID"].IsNumber) { throw new SerializationException(); }  ID = _buf["ID"]; }
        { if(!_buf["day"].IsNumber) { throw new SerializationException(); }  Day = _buf["day"]; }
        { if(!_buf["money"].IsNumber) { throw new SerializationException(); }  Money = _buf["money"]; }
    }

    public static DailyConfig DeserializeDailyConfig(JSONNode _buf)
    {
        return new DailyConfig(_buf);
    }

    /// <summary>
    /// ID
    /// </summary>
    public readonly int ID;
    /// <summary>
    /// 连登天数
    /// </summary>
    public readonly int Day;
    /// <summary>
    /// 奖励倍数
    /// </summary>
    public readonly int Money;
   
    public const int __ID__ = 958021275;
    public override int GetTypeId() => __ID__;

    public  void ResolveRef(Tables tables)
    {
    }

    public override string ToString()
    {
        return "{ "
        + "ID:" + ID + ","
        + "day:" + Day + ","
        + "money:" + Money + ","
        + "}";
    }
}

}
