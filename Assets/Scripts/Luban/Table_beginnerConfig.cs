
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
public partial class Table_beginnerConfig
{
    private readonly System.Collections.Generic.Dictionary<int, levelConfignew_beginnerConfig> _dataMap;
    private readonly System.Collections.Generic.List<levelConfignew_beginnerConfig> _dataList;
    
    public Table_beginnerConfig(JSONNode _buf)
    {
        _dataMap = new System.Collections.Generic.Dictionary<int, levelConfignew_beginnerConfig>();
        _dataList = new System.Collections.Generic.List<levelConfignew_beginnerConfig>();
        
        foreach(JSONNode _ele in _buf.Children)
        {
            levelConfignew_beginnerConfig _v;
            { if(!_ele.IsObject) { throw new SerializationException(); }  _v = levelConfignew_beginnerConfig.DeserializelevelConfignew_beginnerConfig(_ele);  }
            _dataList.Add(_v);
            _dataMap.Add(_v.Id, _v);
        }
    }

    public System.Collections.Generic.Dictionary<int, levelConfignew_beginnerConfig> DataMap => _dataMap;
    public System.Collections.Generic.List<levelConfignew_beginnerConfig> DataList => _dataList;

    public levelConfignew_beginnerConfig GetOrDefault(int key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public levelConfignew_beginnerConfig Get(int key) => _dataMap[key];
    public levelConfignew_beginnerConfig this[int key] => _dataMap[key];

    public void ResolveRef(Tables tables)
    {
        foreach(var _v in _dataList)
        {
            _v.ResolveRef(tables);
        }
    }

}

}

