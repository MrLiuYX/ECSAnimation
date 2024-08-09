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



namespace cfg.heroName
{ 

public sealed partial class HeroName :  Bright.Config.BeanBase 
{
    public HeroName(JSONNode _json) 
    {
        { if(!_json["id"].IsNumber) { throw new SerializationException(); }  Id = _json["id"]; }
        { if(!_json["name"].IsString) { throw new SerializationException(); }  Name = _json["name"]; }
        { if(!_json["desc"].IsString) { throw new SerializationException(); }  Desc = _json["desc"]; }
        { if(!_json["anim"].IsString) { throw new SerializationException(); }  Anim = _json["anim"]; }
        { if(!_json["zhenying"].IsString) { throw new SerializationException(); }  Zhenying = _json["zhenying"]; }
        { if(!_json["defaultWeaponID"].IsString) { throw new SerializationException(); }  DefaultWeaponID = _json["defaultWeaponID"]; }
        { if(!_json["Price"].IsNumber) { throw new SerializationException(); }  Price = _json["Price"]; }
        { if(!_json["HeroStar"].IsNumber) { throw new SerializationException(); }  HeroStar = _json["HeroStar"]; }
        { var __json0 = _json["Skills"]; if(!__json0.IsArray) { throw new SerializationException(); } int _n0 = __json0.Count; Skills = new string[_n0]; int __index0=0; foreach(JSONNode __e0 in __json0.Children) { string __v0;  { if(!__e0.IsString) { throw new SerializationException(); }  __v0 = __e0; }  Skills[__index0++] = __v0; }   }
        PostInit();
    }

    public HeroName(int id, string name, string desc, string anim, string zhenying, string defaultWeaponID, int Price, int HeroStar, string[] Skills ) 
    {
        this.Id = id;
        this.Name = name;
        this.Desc = desc;
        this.Anim = anim;
        this.Zhenying = zhenying;
        this.DefaultWeaponID = defaultWeaponID;
        this.Price = Price;
        this.HeroStar = HeroStar;
        this.Skills = Skills;
        PostInit();
    }

    public static HeroName DeserializeHeroName(JSONNode _json)
    {
        return new heroName.HeroName(_json);
    }

    /// <summary>
    /// 这是id
    /// </summary>
    public int Id { get; private set; }
    /// <summary>
    /// 英雄名称
    /// </summary>
    public string Name { get; private set; }
    /// <summary>
    /// 描述
    /// </summary>
    public string Desc { get; private set; }
    /// <summary>
    /// 动画资源名称
    /// </summary>
    public string Anim { get; private set; }
    /// <summary>
    /// 阵营
    /// </summary>
    public string Zhenying { get; private set; }
    /// <summary>
    /// 默认武器id
    /// </summary>
    public string DefaultWeaponID { get; private set; }
    /// <summary>
    /// 购买价格
    /// </summary>
    public int Price { get; private set; }
    /// <summary>
    /// 英雄星级
    /// </summary>
    public int HeroStar { get; private set; }
    /// <summary>
    /// 技能集合
    /// </summary>
    public string[] Skills { get; private set; }

    public const int __ID__ = -1773969426;
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
        + "Name:" + Name + ","
        + "Desc:" + Desc + ","
        + "Anim:" + Anim + ","
        + "Zhenying:" + Zhenying + ","
        + "DefaultWeaponID:" + DefaultWeaponID + ","
        + "Price:" + Price + ","
        + "HeroStar:" + HeroStar + ","
        + "Skills:" + Bright.Common.StringUtil.CollectionToString(Skills) + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
