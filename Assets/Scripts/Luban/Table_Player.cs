
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
public partial class Table_Player
{
    private readonly System.Collections.Generic.Dictionary<int, PlayerConfignew_Player> _dataMap;
    private readonly System.Collections.Generic.List<PlayerConfignew_Player> _dataList;
    
    public Table_Player(JSONNode _buf)
    {
        _dataMap = new System.Collections.Generic.Dictionary<int, PlayerConfignew_Player>();
        _dataList = new System.Collections.Generic.List<PlayerConfignew_Player>();
        
        foreach(JSONNode _ele in _buf.Children)
        {
            PlayerConfignew_Player _v;
            { if(!_ele.IsObject) { throw new SerializationException(); }  _v = PlayerConfignew_Player.DeserializePlayerConfignew_Player(_ele);  }
            _dataList.Add(_v);
            _dataMap.Add(_v.Lv, _v);
        }
    }

    public System.Collections.Generic.Dictionary<int, PlayerConfignew_Player> DataMap => _dataMap;
    public System.Collections.Generic.List<PlayerConfignew_Player> DataList => _dataList;

    public PlayerConfignew_Player GetOrDefault(int key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public PlayerConfignew_Player Get(int key) => _dataMap[key];
    public PlayerConfignew_Player this[int key] => _dataMap[key];

    public void ResolveRef(Tables tables)
    {
        foreach(var _v in _dataList)
        {
            _v.ResolveRef(tables);
        }
    }

}

}

