
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
public partial class Table_PhysiqueReslevelConfig
{
    private readonly System.Collections.Generic.Dictionary<int, levelConfig_PhysiqueRes> _dataMap;
    private readonly System.Collections.Generic.List<levelConfig_PhysiqueRes> _dataList;
    
    public Table_PhysiqueReslevelConfig(JSONNode _buf)
    {
        _dataMap = new System.Collections.Generic.Dictionary<int, levelConfig_PhysiqueRes>();
        _dataList = new System.Collections.Generic.List<levelConfig_PhysiqueRes>();
        
        foreach(JSONNode _ele in _buf.Children)
        {
            levelConfig_PhysiqueRes _v;
            { if(!_ele.IsObject) { throw new SerializationException(); }  _v = levelConfig_PhysiqueRes.DeserializelevelConfig_PhysiqueRes(_ele);  }
            _dataList.Add(_v);
            _dataMap.Add(_v.Id, _v);
        }
    }

    public System.Collections.Generic.Dictionary<int, levelConfig_PhysiqueRes> DataMap => _dataMap;
    public System.Collections.Generic.List<levelConfig_PhysiqueRes> DataList => _dataList;

    public levelConfig_PhysiqueRes GetOrDefault(int key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public levelConfig_PhysiqueRes Get(int key) => _dataMap[key];
    public levelConfig_PhysiqueRes this[int key] => _dataMap[key];

    public void ResolveRef(Tables tables)
    {
        foreach(var _v in _dataList)
        {
            _v.ResolveRef(tables);
        }
    }

}

}

