
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
public sealed partial class levelConfignew_beginnerConfig : Luban.BeanBase
{
    public levelConfignew_beginnerConfig(JSONNode _buf) 
    {
        { if(!_buf["id"].IsNumber) { throw new SerializationException(); }  Id = _buf["id"]; }
        { if(!_buf["level"].IsNumber) { throw new SerializationException(); }  Level = _buf["level"]; }
        { if(!_buf["name"].IsString) { throw new SerializationException(); }  Name = _buf["name"]; }
        { if(!_buf["resource"].IsString) { throw new SerializationException(); }  Resource = _buf["resource"]; }
        { if(!_buf["time"].IsNumber) { throw new SerializationException(); }  Time = _buf["time"]; }
        { var __json0 = _buf["monster1"]; if(!__json0.IsArray) { throw new SerializationException(); } Monster1 = new System.Collections.Generic.List<int>(__json0.Count); foreach(JSONNode __e0 in __json0.Children) { int __v0;  { if(!__e0.IsNumber) { throw new SerializationException(); }  __v0 = __e0; }  Monster1.Add(__v0); }   }
        { if(!_buf["quantity_min1"].IsNumber) { throw new SerializationException(); }  QuantityMin1 = _buf["quantity_min1"]; }
        { if(!_buf["quantity_max1"].IsNumber) { throw new SerializationException(); }  QuantityMax1 = _buf["quantity_max1"]; }
        { var __json0 = _buf["monster2"]; if(!__json0.IsArray) { throw new SerializationException(); } Monster2 = new System.Collections.Generic.List<int>(__json0.Count); foreach(JSONNode __e0 in __json0.Children) { int __v0;  { if(!__e0.IsNumber) { throw new SerializationException(); }  __v0 = __e0; }  Monster2.Add(__v0); }   }
        { if(!_buf["quantity_min2"].IsNumber) { throw new SerializationException(); }  QuantityMin2 = _buf["quantity_min2"]; }
        { if(!_buf["quantity_max2"].IsNumber) { throw new SerializationException(); }  QuantityMax2 = _buf["quantity_max2"]; }
        { var __json0 = _buf["monster3"]; if(!__json0.IsArray) { throw new SerializationException(); } Monster3 = new System.Collections.Generic.List<int>(__json0.Count); foreach(JSONNode __e0 in __json0.Children) { int __v0;  { if(!__e0.IsNumber) { throw new SerializationException(); }  __v0 = __e0; }  Monster3.Add(__v0); }   }
        { if(!_buf["quantity_min3"].IsNumber) { throw new SerializationException(); }  QuantityMin3 = _buf["quantity_min3"]; }
        { if(!_buf["quantity_max3"].IsNumber) { throw new SerializationException(); }  QuantityMax3 = _buf["quantity_max3"]; }
        { var __json0 = _buf["monster4"]; if(!__json0.IsArray) { throw new SerializationException(); } Monster4 = new System.Collections.Generic.List<int>(__json0.Count); foreach(JSONNode __e0 in __json0.Children) { int __v0;  { if(!__e0.IsNumber) { throw new SerializationException(); }  __v0 = __e0; }  Monster4.Add(__v0); }   }
        { if(!_buf["quantity_min4"].IsNumber) { throw new SerializationException(); }  QuantityMin4 = _buf["quantity_min4"]; }
        { if(!_buf["quantity_max4"].IsNumber) { throw new SerializationException(); }  QuantityMax4 = _buf["quantity_max4"]; }
        { if(!_buf["note1"].IsString) { throw new SerializationException(); }  Note1 = _buf["note1"]; }
        { if(!_buf["note2"].IsString) { throw new SerializationException(); }  Note2 = _buf["note2"]; }
    }

    public static levelConfignew_beginnerConfig DeserializelevelConfignew_beginnerConfig(JSONNode _buf)
    {
        return new levelConfignew_beginnerConfig(_buf);
    }

    /// <summary>
    /// 配置id
    /// </summary>
    public readonly int Id;
    /// <summary>
    /// 关卡ID
    /// </summary>
    public readonly int Level;
    /// <summary>
    /// 名称
    /// </summary>
    public readonly string Name;
    /// <summary>
    /// 背景
    /// </summary>
    public readonly string Resource;
    /// <summary>
    /// 波时间（ms）
    /// </summary>
    public readonly int Time;
    /// <summary>
    /// 怪物1（普通僵尸）
    /// </summary>
    public readonly System.Collections.Generic.List<int> Monster1;
    /// <summary>
    /// 怪物1数量下限
    /// </summary>
    public readonly float QuantityMin1;
    /// <summary>
    /// 怪物1数量上限
    /// </summary>
    public readonly float QuantityMax1;
    /// <summary>
    /// 怪物2（篮球僵尸）
    /// </summary>
    public readonly System.Collections.Generic.List<int> Monster2;
    /// <summary>
    /// 怪物2数量下限
    /// </summary>
    public readonly float QuantityMin2;
    /// <summary>
    /// 怪物2数量上限
    /// </summary>
    public readonly float QuantityMax2;
    /// <summary>
    /// 怪物3（钢铁僵尸）
    /// </summary>
    public readonly System.Collections.Generic.List<int> Monster3;
    /// <summary>
    /// 怪物3数量下限
    /// </summary>
    public readonly float QuantityMin3;
    /// <summary>
    /// 怪物3数量上限
    /// </summary>
    public readonly float QuantityMax3;
    /// <summary>
    /// 怪物4（绿巨人）
    /// </summary>
    public readonly System.Collections.Generic.List<int> Monster4;
    /// <summary>
    /// 怪物4数量下限
    /// </summary>
    public readonly float QuantityMin4;
    /// <summary>
    /// 怪物4数量上限
    /// </summary>
    public readonly float QuantityMax4;
    /// <summary>
    /// 是否BOSS关
    /// </summary>
    public readonly string Note1;
    /// <summary>
    /// 新手关备注说明
    /// </summary>
    public readonly string Note2;
   
    public const int __ID__ = -173135591;
    public override int GetTypeId() => __ID__;

    public  void ResolveRef(Tables tables)
    {
    }

    public override string ToString()
    {
        return "{ "
        + "id:" + Id + ","
        + "level:" + Level + ","
        + "name:" + Name + ","
        + "resource:" + Resource + ","
        + "time:" + Time + ","
        + "monster1:" + Luban.StringUtil.CollectionToString(Monster1) + ","
        + "quantityMin1:" + QuantityMin1 + ","
        + "quantityMax1:" + QuantityMax1 + ","
        + "monster2:" + Luban.StringUtil.CollectionToString(Monster2) + ","
        + "quantityMin2:" + QuantityMin2 + ","
        + "quantityMax2:" + QuantityMax2 + ","
        + "monster3:" + Luban.StringUtil.CollectionToString(Monster3) + ","
        + "quantityMin3:" + QuantityMin3 + ","
        + "quantityMax3:" + QuantityMax3 + ","
        + "monster4:" + Luban.StringUtil.CollectionToString(Monster4) + ","
        + "quantityMin4:" + QuantityMin4 + ","
        + "quantityMax4:" + QuantityMax4 + ","
        + "note1:" + Note1 + ","
        + "note2:" + Note2 + ","
        + "}";
    }
}

}

