
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
public sealed partial class skillConfig_BuffRes : Luban.BeanBase
{
    public skillConfig_BuffRes(JSONNode _buf) 
    {
        { if(!_buf["id"].IsNumber) { throw new SerializationException(); }  Id = _buf["id"]; }
        { if(!_buf["name"].IsString) { throw new SerializationException(); }  Name = _buf["name"]; }
        { if(!_buf["des"].IsString) { throw new SerializationException(); }  Des = _buf["des"]; }
        { if(!_buf["icon"].IsString) { throw new SerializationException(); }  Icon = _buf["icon"]; }
        { if(!_buf["sound"].IsString) { throw new SerializationException(); }  Sound = _buf["sound"]; }
        { if(!_buf["cd"].IsNumber) { throw new SerializationException(); }  Cd = _buf["cd"]; }
        { var __json0 = _buf["attributes"]; if(!__json0.IsArray) { throw new SerializationException(); } Attributes = new System.Collections.Generic.List<int>(__json0.Count); foreach(JSONNode __e0 in __json0.Children) { int __v0;  { if(!__e0.IsNumber) { throw new SerializationException(); }  __v0 = __e0; }  Attributes.Add(__v0); }   }
        { if(!_buf["isForever"].IsNumber) { throw new SerializationException(); }  IsForever = _buf["isForever"]; }
        { if(!_buf["effect"].IsNumber) { throw new SerializationException(); }  Effect = _buf["effect"]; }
        { var __json0 = _buf["ownSkills"]; if(!__json0.IsArray) { throw new SerializationException(); } OwnSkills = new System.Collections.Generic.List<int>(__json0.Count); foreach(JSONNode __e0 in __json0.Children) { int __v0;  { if(!__e0.IsNumber) { throw new SerializationException(); }  __v0 = __e0; }  OwnSkills.Add(__v0); }   }
        { var __json0 = _buf["hpChange"]; if(!__json0.IsArray) { throw new SerializationException(); } HpChange = new System.Collections.Generic.List<float>(__json0.Count); foreach(JSONNode __e0 in __json0.Children) { float __v0;  { if(!__e0.IsNumber) { throw new SerializationException(); }  __v0 = __e0; }  HpChange.Add(__v0); }   }
        { if(!_buf["livesTimes"].IsNumber) { throw new SerializationException(); }  LivesTimes = _buf["livesTimes"]; }
        { if(!_buf["mergeTimes"].IsNumber) { throw new SerializationException(); }  MergeTimes = _buf["mergeTimes"]; }
        { if(!_buf["mergeType"].IsNumber) { throw new SerializationException(); }  MergeType = _buf["mergeType"]; }
    }

    public static skillConfig_BuffRes DeserializeskillConfig_BuffRes(JSONNode _buf)
    {
        return new skillConfig_BuffRes(_buf);
    }

    /// <summary>
    /// 配置id
    /// </summary>
    public readonly int Id;
    /// <summary>
    /// 名称
    /// </summary>
    public readonly string Name;
    /// <summary>
    /// 描述
    /// </summary>
    public readonly string Des;
    /// <summary>
    /// 图标
    /// </summary>
    public readonly string Icon;
    /// <summary>
    /// 释放音效
    /// </summary>
    public readonly string Sound;
    /// <summary>
    /// 冷却时间（ms）
    /// </summary>
    public readonly int Cd;
    /// <summary>
    /// 属性数组
    /// </summary>
    public readonly System.Collections.Generic.List<int> Attributes;
    /// <summary>
    /// 是否永久
    /// </summary>
    public readonly int IsForever;
    /// <summary>
    /// 拥有时显示特效
    /// </summary>
    public readonly int Effect;
    /// <summary>
    /// 激活时给自己加SKILL
    /// </summary>
    public readonly System.Collections.Generic.List<int> OwnSkills;
    /// <summary>
    /// 每次生效时血量变化:1.回血2.伤害
    /// </summary>
    public readonly System.Collections.Generic.List<float> HpChange;
    /// <summary>
    /// 生效次数
    /// </summary>
    public readonly int LivesTimes;
    /// <summary>
    /// 最多叠加次数（相同id）
    /// </summary>
    public readonly int MergeTimes;
    /// <summary>
    /// 叠加方式（相同id）
    /// </summary>
    public readonly int MergeType;
   
    public const int __ID__ = -1264533535;
    public override int GetTypeId() => __ID__;

    public  void ResolveRef(Tables tables)
    {
    }

    public override string ToString()
    {
        return "{ "
        + "id:" + Id + ","
        + "name:" + Name + ","
        + "des:" + Des + ","
        + "icon:" + Icon + ","
        + "sound:" + Sound + ","
        + "cd:" + Cd + ","
        + "attributes:" + Luban.StringUtil.CollectionToString(Attributes) + ","
        + "isForever:" + IsForever + ","
        + "effect:" + Effect + ","
        + "ownSkills:" + Luban.StringUtil.CollectionToString(OwnSkills) + ","
        + "hpChange:" + Luban.StringUtil.CollectionToString(HpChange) + ","
        + "livesTimes:" + LivesTimes + ","
        + "mergeTimes:" + MergeTimes + ","
        + "mergeType:" + MergeType + ","
        + "}";
    }
}

}

