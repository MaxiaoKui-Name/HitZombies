
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
public partial class Table_Doorgenerate
{
    private readonly System.Collections.Generic.Dictionary<int, DoorRes_Doorgenerate> _dataMap;
    private readonly System.Collections.Generic.List<DoorRes_Doorgenerate> _dataList;
    
    public Table_Doorgenerate(JSONNode _buf)
    {
        _dataMap = new System.Collections.Generic.Dictionary<int, DoorRes_Doorgenerate>();
        _dataList = new System.Collections.Generic.List<DoorRes_Doorgenerate>();
        
        foreach(JSONNode _ele in _buf.Children)
        {
            DoorRes_Doorgenerate _v;
            { if(!_ele.IsObject) { throw new SerializationException(); }  _v = DoorRes_Doorgenerate.DeserializeDoorRes_Doorgenerate(_ele);  }
            _dataList.Add(_v);
            _dataMap.Add(_v.Id, _v);
        }
    }

    public System.Collections.Generic.Dictionary<int, DoorRes_Doorgenerate> DataMap => _dataMap;
    public System.Collections.Generic.List<DoorRes_Doorgenerate> DataList => _dataList;

    public DoorRes_Doorgenerate GetOrDefault(int key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public DoorRes_Doorgenerate Get(int key) => _dataMap[key];
    public DoorRes_Doorgenerate this[int key] => _dataMap[key];

    public void ResolveRef(Tables tables)
    {
        foreach(var _v in _dataList)
        {
            _v.ResolveRef(tables);
        }
    }

}

}

