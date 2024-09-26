
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
public sealed partial class transmitConfig_transmitConfig : Luban.BeanBase
{
    public transmitConfig_transmitConfig(JSONNode _buf) 
    {
        { if(!_buf["id"].IsNumber) { throw new SerializationException(); }  Id = _buf["id"]; }
        { if(!_buf["note"].IsString) { throw new SerializationException(); }  Note = _buf["note"]; }
        { if(!_buf["strategy"].IsNumber) { throw new SerializationException(); }  Strategy = _buf["strategy"]; }
        { var __json0 = _buf["strategyParams"]; if(!__json0.IsArray) { throw new SerializationException(); } StrategyParams = new System.Collections.Generic.List<float>(__json0.Count); foreach(JSONNode __e0 in __json0.Children) { float __v0;  { if(!__e0.IsNumber) { throw new SerializationException(); }  __v0 = __e0; }  StrategyParams.Add(__v0); }   }
        { if(!_buf["sound1"].IsString) { throw new SerializationException(); }  Sound1 = _buf["sound1"]; }
        { if(!_buf["delay"].IsNumber) { throw new SerializationException(); }  Delay = _buf["delay"]; }
        { if(!_buf["atkRate"].IsNumber) { throw new SerializationException(); }  AtkRate = _buf["atkRate"]; }
        { if(!_buf["atkBack"].IsNumber) { throw new SerializationException(); }  AtkBack = _buf["atkBack"]; }
        { if(!_buf["resource"].IsString) { throw new SerializationException(); }  Resource = _buf["resource"]; }
        { if(!_buf["sound2"].IsString) { throw new SerializationException(); }  Sound2 = _buf["sound2"]; }
        { if(!_buf["DamageScope"].IsNumber) { throw new SerializationException(); }  DamageScope = _buf["DamageScope"]; }
        { if(!_buf["isGroupAttack"].IsBoolean) { throw new SerializationException(); }  IsGroupAttack = _buf["isGroupAttack"]; }
        { if(!_buf["isCircleBack"].IsBoolean) { throw new SerializationException(); }  IsCircleBack = _buf["isCircleBack"]; }
        { if(!_buf["isGround"].IsBoolean) { throw new SerializationException(); }  IsGround = _buf["isGround"]; }
        { if(!_buf["isRotation"].IsBoolean) { throw new SerializationException(); }  IsRotation = _buf["isRotation"]; }
        { if(!_buf["isPassActor"].IsBoolean) { throw new SerializationException(); }  IsPassActor = _buf["isPassActor"]; }
    }

    public static transmitConfig_transmitConfig DeserializetransmitConfig_transmitConfig(JSONNode _buf)
    {
        return new transmitConfig_transmitConfig(_buf);
    }

    /// <summary>
    /// 配置id
    /// </summary>
    public readonly int Id;
    /// <summary>
    /// 备注
    /// </summary>
    public readonly string Note;
    /// <summary>
    /// 发射方式
    /// </summary>
    public readonly int Strategy;
    /// <summary>
    /// 发射参数
    /// </summary>
    public readonly System.Collections.Generic.List<float> StrategyParams;
    /// <summary>
    /// 释放音效
    /// </summary>
    public readonly string Sound1;
    /// <summary>
    /// 发射延迟
    /// </summary>
    public readonly int Delay;
    /// <summary>
    /// 攻击系数
    /// </summary>
    public readonly float AtkRate;
    /// <summary>
    /// 击退力度（10代表击退体重100的怪10像素，击退体重50的怪20像素，5代表击退体重100的怪5像素，击退体重20的怪25像素，以此类推）
    /// </summary>
    public readonly int AtkBack;
    /// <summary>
    /// 子弹美术资源
    /// </summary>
    public readonly string Resource;
    /// <summary>
    /// 子弹释放音效
    /// </summary>
    public readonly string Sound2;
    /// <summary>
    /// 子弹伤害范围
    /// </summary>
    public readonly int DamageScope;
    /// <summary>
    /// 子弹是否群攻
    /// </summary>
    public readonly bool IsGroupAttack;
    /// <summary>
    /// 子弹是否圆周击退
    /// </summary>
    public readonly bool IsCircleBack;
    /// <summary>
    /// 子弹是否贴地
    /// </summary>
    public readonly bool IsGround;
    /// <summary>
    /// 子弹是否跟随方向旋转
    /// </summary>
    public readonly bool IsRotation;
    /// <summary>
    /// 子弹是否穿人
    /// </summary>
    public readonly bool IsPassActor;
   
    public const int __ID__ = -2082322465;
    public override int GetTypeId() => __ID__;

    public  void ResolveRef(Tables tables)
    {
    }

    public override string ToString()
    {
        return "{ "
        + "id:" + Id + ","
        + "note:" + Note + ","
        + "strategy:" + Strategy + ","
        + "strategyParams:" + Luban.StringUtil.CollectionToString(StrategyParams) + ","
        + "sound1:" + Sound1 + ","
        + "delay:" + Delay + ","
        + "atkRate:" + AtkRate + ","
        + "atkBack:" + AtkBack + ","
        + "resource:" + Resource + ","
        + "sound2:" + Sound2 + ","
        + "DamageScope:" + DamageScope + ","
        + "isGroupAttack:" + IsGroupAttack + ","
        + "isCircleBack:" + IsCircleBack + ","
        + "isGround:" + IsGround + ","
        + "isRotation:" + IsRotation + ","
        + "isPassActor:" + IsPassActor + ","
        + "}";
    }
}

}
