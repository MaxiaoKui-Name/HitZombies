
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
public partial class Table_LanguagelanguageConfig
{
    private readonly System.Collections.Generic.Dictionary<string, languageConfig_Language> _dataMap;
    private readonly System.Collections.Generic.List<languageConfig_Language> _dataList;
    
    public Table_LanguagelanguageConfig(JSONNode _buf)
    {
        _dataMap = new System.Collections.Generic.Dictionary<string, languageConfig_Language>();
        _dataList = new System.Collections.Generic.List<languageConfig_Language>();
        
        foreach(JSONNode _ele in _buf.Children)
        {
            languageConfig_Language _v;
            { if(!_ele.IsObject) { throw new SerializationException(); }  _v = languageConfig_Language.DeserializelanguageConfig_Language(_ele);  }
            _dataList.Add(_v);
            _dataMap.Add(_v.Keyname, _v);
        }
    }

    public System.Collections.Generic.Dictionary<string, languageConfig_Language> DataMap => _dataMap;
    public System.Collections.Generic.List<languageConfig_Language> DataList => _dataList;

    public languageConfig_Language GetOrDefault(string key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public languageConfig_Language Get(string key) => _dataMap[key];
    public languageConfig_Language this[string key] => _dataMap[key];

    public void ResolveRef(Tables tables)
    {
        foreach(var _v in _dataList)
        {
            _v.ResolveRef(tables);
        }
    }

}

}

