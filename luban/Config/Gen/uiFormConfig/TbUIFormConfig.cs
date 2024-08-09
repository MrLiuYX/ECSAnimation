//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Bright.Serialization;
using System.Collections.Generic;
using SimpleJSON;



namespace cfg.uiFormConfig
{ 

public sealed partial class TbUIFormConfig
{
    private readonly Dictionary<int, uiFormConfig.UIFormConfig> _dataMap;
    private readonly List<uiFormConfig.UIFormConfig> _dataList;
    
    public TbUIFormConfig(JSONNode _json)
    {
        _dataMap = new Dictionary<int, uiFormConfig.UIFormConfig>();
        _dataList = new List<uiFormConfig.UIFormConfig>();
        
        foreach(JSONNode _row in _json.Children)
        {
            var _v = uiFormConfig.UIFormConfig.DeserializeUIFormConfig(_row);
            _dataList.Add(_v);
            _dataMap.Add(_v.Id, _v);
        }
        PostInit();
    }

    public Dictionary<int, uiFormConfig.UIFormConfig> DataMap => _dataMap;
    public List<uiFormConfig.UIFormConfig> DataList => _dataList;

    public uiFormConfig.UIFormConfig GetOrDefault(int key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public uiFormConfig.UIFormConfig Get(int key) => _dataMap[key];
    public uiFormConfig.UIFormConfig this[int key] => _dataMap[key];

    public void Resolve(Dictionary<string, object> _tables)
    {
        foreach(var v in _dataList)
        {
            v.Resolve(_tables);
        }
        PostResolve();
    }

    public void TranslateText(System.Func<string, string, string> translator)
    {
        foreach(var v in _dataList)
        {
            v.TranslateText(translator);
        }
    }
    
    
    partial void PostInit();
    partial void PostResolve();
}

}