
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
public partial class Table_languageConfig
{
    private readonly System.Collections.Generic.Dictionary<string, languageConfignew_languageConfig> _dataMap;
    private readonly System.Collections.Generic.List<languageConfignew_languageConfig> _dataList;
    
    public Table_languageConfig(JSONNode _buf)
    {
        _dataMap = new System.Collections.Generic.Dictionary<string, languageConfignew_languageConfig>();
        _dataList = new System.Collections.Generic.List<languageConfignew_languageConfig>();
        
        foreach(JSONNode _ele in _buf.Children)
        {
            languageConfignew_languageConfig _v;
            { if(!_ele.IsObject) { throw new SerializationException(); }  _v = languageConfignew_languageConfig.DeserializelanguageConfignew_languageConfig(_ele);  }
            _dataList.Add(_v);
            _dataMap.Add(_v.Keyname, _v);
        }
    }

    public System.Collections.Generic.Dictionary<string, languageConfignew_languageConfig> DataMap => _dataMap;
    public System.Collections.Generic.List<languageConfignew_languageConfig> DataList => _dataList;

    public languageConfignew_languageConfig GetOrDefault(string key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public languageConfignew_languageConfig Get(string key) => _dataMap[key];
    public languageConfignew_languageConfig this[string key] => _dataMap[key];

    public void ResolveRef(Tables tables)
    {
        foreach(var _v in _dataList)
        {
            _v.ResolveRef(tables);
        }
    }

}

}
