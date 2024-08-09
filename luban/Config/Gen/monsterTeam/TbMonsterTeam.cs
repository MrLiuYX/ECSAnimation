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



namespace cfg.monsterTeam
{ 

public sealed partial class TbMonsterTeam
{
    private readonly Dictionary<string, monsterTeam.MonsterTeam> _dataMap;
    private readonly List<monsterTeam.MonsterTeam> _dataList;
    
    public TbMonsterTeam(JSONNode _json)
    {
        _dataMap = new Dictionary<string, monsterTeam.MonsterTeam>();
        _dataList = new List<monsterTeam.MonsterTeam>();
        
        foreach(JSONNode _row in _json.Children)
        {
            var _v = monsterTeam.MonsterTeam.DeserializeMonsterTeam(_row);
            _dataList.Add(_v);
            _dataMap.Add(_v.Id, _v);
        }
        PostInit();
    }

    public Dictionary<string, monsterTeam.MonsterTeam> DataMap => _dataMap;
    public List<monsterTeam.MonsterTeam> DataList => _dataList;

    public monsterTeam.MonsterTeam GetOrDefault(string key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public monsterTeam.MonsterTeam Get(string key) => _dataMap[key];
    public monsterTeam.MonsterTeam this[string key] => _dataMap[key];

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