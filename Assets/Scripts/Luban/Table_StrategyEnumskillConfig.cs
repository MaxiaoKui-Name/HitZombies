
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
public partial class Table_StrategyEnumskillConfig
{
    private readonly System.Collections.Generic.Dictionary<string, skillConfig_StrategyEnum> _dataMap;
    private readonly System.Collections.Generic.List<skillConfig_StrategyEnum> _dataList;
    
    public Table_StrategyEnumskillConfig(JSONNode _buf)
    {
        _dataMap = new System.Collections.Generic.Dictionary<string, skillConfig_StrategyEnum>();
        _dataList = new System.Collections.Generic.List<skillConfig_StrategyEnum>();
        
        foreach(JSONNode _ele in _buf.Children)
        {
            skillConfig_StrategyEnum _v;
            { if(!_ele.IsObject) { throw new SerializationException(); }  _v = skillConfig_StrategyEnum.DeserializeskillConfig_StrategyEnum(_ele);  }
            _dataList.Add(_v);
            _dataMap.Add(_v.EnumId, _v);
        }
    }

    public System.Collections.Generic.Dictionary<string, skillConfig_StrategyEnum> DataMap => _dataMap;
    public System.Collections.Generic.List<skillConfig_StrategyEnum> DataList => _dataList;

    public skillConfig_StrategyEnum GetOrDefault(string key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public skillConfig_StrategyEnum Get(string key) => _dataMap[key];
    public skillConfig_StrategyEnum this[string key] => _dataMap[key];

    public void ResolveRef(Tables tables)
    {
        foreach(var _v in _dataList)
        {
            _v.ResolveRef(tables);
        }
    }

}

}

