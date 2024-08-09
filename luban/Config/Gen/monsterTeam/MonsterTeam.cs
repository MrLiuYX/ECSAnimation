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

public sealed partial class MonsterTeam :  Bright.Config.BeanBase 
{
    public MonsterTeam(JSONNode _json) 
    {
        { if(!_json["id"].IsString) { throw new SerializationException(); }  Id = _json["id"]; }
        { var __json0 = _json["RefreshOrder"]; if(!__json0.IsArray) { throw new SerializationException(); } int _n0 = __json0.Count; RefreshOrder = new float[_n0][]; int __index0=0; foreach(JSONNode __e0 in __json0.Children) { float[] __v0;  { var __json1 = __e0; if(!__json1.IsArray) { throw new SerializationException(); } int _n1 = __json1.Count; __v0 = new float[_n1]; int __index1=0; foreach(JSONNode __e1 in __json1.Children) { float __v1;  { if(!__e1.IsNumber) { throw new SerializationException(); }  __v1 = __e1; }  __v0[__index1++] = __v1; }   }  RefreshOrder[__index0++] = __v0; }   }
        PostInit();
    }

    public MonsterTeam(string id, float[][] RefreshOrder ) 
    {
        this.Id = id;
        this.RefreshOrder = RefreshOrder;
        PostInit();
    }

    public static MonsterTeam DeserializeMonsterTeam(JSONNode _json)
    {
        return new monsterTeam.MonsterTeam(_json);
    }

    /// <summary>
    /// 这是id
    /// </summary>
    public string Id { get; private set; }
    /// <summary>
    /// 刷新顺序(1=id,2=刷数量,3=该怪刷新CD,4=下一波CD,5=怪物是死亡刷新1，还是等时间刷新0)
    /// </summary>
    public float[][] RefreshOrder { get; private set; }

    public const int __ID__ = -932833504;
    public override int GetTypeId() => __ID__;

    public  void Resolve(Dictionary<string, object> _tables)
    {
        PostResolve();
    }

    public  void TranslateText(System.Func<string, string, string> translator)
    {
    }

    public override string ToString()
    {
        return "{ "
        + "Id:" + Id + ","
        + "RefreshOrder:" + Bright.Common.StringUtil.CollectionToString(RefreshOrder) + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}