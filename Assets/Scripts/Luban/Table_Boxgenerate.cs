
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
public partial class Table_Boxgenerate
{
    private readonly System.Collections.Generic.Dictionary<int, BoxRes_Boxgenerate> _dataMap;
    private readonly System.Collections.Generic.List<BoxRes_Boxgenerate> _dataList;
    
    public Table_Boxgenerate(JSONNode _buf)
    {
        _dataMap = new System.Collections.Generic.Dictionary<int, BoxRes_Boxgenerate>();
        _dataList = new System.Collections.Generic.List<BoxRes_Boxgenerate>();
        
        foreach(JSONNode _ele in _buf.Children)
        {
            BoxRes_Boxgenerate _v;
            { if(!_ele.IsObject) { throw new SerializationException(); }  _v = BoxRes_Boxgenerate.DeserializeBoxRes_Boxgenerate(_ele);  }
            _dataList.Add(_v);
            _dataMap.Add(_v.Id, _v);
        }
    }

    public System.Collections.Generic.Dictionary<int, BoxRes_Boxgenerate> DataMap => _dataMap;
    public System.Collections.Generic.List<BoxRes_Boxgenerate> DataList => _dataList;

    public BoxRes_Boxgenerate GetOrDefault(int key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public BoxRes_Boxgenerate Get(int key) => _dataMap[key];
    public BoxRes_Boxgenerate this[int key] => _dataMap[key];

    public void ResolveRef(Tables tables)
    {
        foreach(var _v in _dataList)
        {
            _v.ResolveRef(tables);
        }
    }

}

}

