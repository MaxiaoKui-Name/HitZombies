
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
public partial class Table_PlayerConfig
{
    private readonly System.Collections.Generic.Dictionary<int, PlayerConfignew_PlayerConfig> _dataMap;
    private readonly System.Collections.Generic.List<PlayerConfignew_PlayerConfig> _dataList;
    
    public Table_PlayerConfig(JSONNode _buf)
    {
        _dataMap = new System.Collections.Generic.Dictionary<int, PlayerConfignew_PlayerConfig>();
        _dataList = new System.Collections.Generic.List<PlayerConfignew_PlayerConfig>();
        
        foreach(JSONNode _ele in _buf.Children)
        {
            PlayerConfignew_PlayerConfig _v;
            { if(!_ele.IsObject) { throw new SerializationException(); }  _v = PlayerConfignew_PlayerConfig.DeserializePlayerConfignew_PlayerConfig(_ele);  }
            _dataList.Add(_v);
            _dataMap.Add(_v.Lv, _v);
        }
    }

    public System.Collections.Generic.Dictionary<int, PlayerConfignew_PlayerConfig> DataMap => _dataMap;
    public System.Collections.Generic.List<PlayerConfignew_PlayerConfig> DataList => _dataList;

    public PlayerConfignew_PlayerConfig GetOrDefault(int key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public PlayerConfignew_PlayerConfig Get(int key) => _dataMap[key];
    public PlayerConfignew_PlayerConfig this[int key] => _dataMap[key];

    public void ResolveRef(Tables tables)
    {
        foreach(var _v in _dataList)
        {
            _v.ResolveRef(tables);
        }
    }

}

}
