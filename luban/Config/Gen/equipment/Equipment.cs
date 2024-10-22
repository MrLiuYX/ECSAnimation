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



namespace cfg.equipment
{ 

public sealed partial class Equipment :  Bright.Config.BeanBase 
{
    public Equipment(JSONNode _json) 
    {
        { if(!_json["id"].IsString) { throw new SerializationException(); }  Id = _json["id"]; }
        { if(!_json["name"].IsString) { throw new SerializationException(); }  Name = _json["name"]; }
        { if(!_json["desc"].IsString) { throw new SerializationException(); }  Desc = _json["desc"]; }
        { if(!_json["icon"].IsString) { throw new SerializationException(); }  Icon = _json["icon"]; }
        { if(!_json["weizhi"].IsString) { throw new SerializationException(); }  Weizhi = _json["weizhi"]; }
        { if(!_json["weaponType"].IsString) { throw new SerializationException(); }  WeaponType = _json["weaponType"]; }
        { if(!_json["HP"].IsNumber) { throw new SerializationException(); }  HP = _json["HP"]; }
        { if(!_json["Att"].IsNumber) { throw new SerializationException(); }  Att = _json["Att"]; }
        { if(!_json["Speed"].IsNumber) { throw new SerializationException(); }  Speed = _json["Speed"]; }
        { if(!_json["Armor"].IsNumber) { throw new SerializationException(); }  Armor = _json["Armor"]; }
        { if(!_json["ArmorMagic"].IsNumber) { throw new SerializationException(); }  ArmorMagic = _json["ArmorMagic"]; }
        { if(!_json["PassAtt"].IsNumber) { throw new SerializationException(); }  PassAtt = _json["PassAtt"]; }
        { if(!_json["PassMagic"].IsNumber) { throw new SerializationException(); }  PassMagic = _json["PassMagic"]; }
        { if(!_json["SkillCD"].IsNumber) { throw new SerializationException(); }  SkillCD = _json["SkillCD"]; }
        { if(!_json["Baoji"].IsNumber) { throw new SerializationException(); }  Baoji = _json["Baoji"]; }
        { if(!_json["BaojiShanghai"].IsNumber) { throw new SerializationException(); }  BaojiShanghai = _json["BaojiShanghai"]; }
        { if(!_json["Shanbi"].IsNumber) { throw new SerializationException(); }  Shanbi = _json["Shanbi"]; }
        { if(!_json["Mingzhong"].IsNumber) { throw new SerializationException(); }  Mingzhong = _json["Mingzhong"]; }
        { if(!_json["Tizhong"].IsNumber) { throw new SerializationException(); }  Tizhong = _json["Tizhong"]; }
        { if(!_json["RemoteAttCD"].IsNumber) { throw new SerializationException(); }  RemoteAttCD = _json["RemoteAttCD"]; }
        PostInit();
    }

    public Equipment(string id, string name, string desc, string icon, string weizhi, string weaponType, int HP, int Att, int Speed, int Armor, int ArmorMagic, int PassAtt, int PassMagic, int SkillCD, int Baoji, int BaojiShanghai, int Shanbi, int Mingzhong, int Tizhong, int RemoteAttCD ) 
    {
        this.Id = id;
        this.Name = name;
        this.Desc = desc;
        this.Icon = icon;
        this.Weizhi = weizhi;
        this.WeaponType = weaponType;
        this.HP = HP;
        this.Att = Att;
        this.Speed = Speed;
        this.Armor = Armor;
        this.ArmorMagic = ArmorMagic;
        this.PassAtt = PassAtt;
        this.PassMagic = PassMagic;
        this.SkillCD = SkillCD;
        this.Baoji = Baoji;
        this.BaojiShanghai = BaojiShanghai;
        this.Shanbi = Shanbi;
        this.Mingzhong = Mingzhong;
        this.Tizhong = Tizhong;
        this.RemoteAttCD = RemoteAttCD;
        PostInit();
    }

    public static Equipment DeserializeEquipment(JSONNode _json)
    {
        return new equipment.Equipment(_json);
    }

    /// <summary>
    /// 这是id
    /// </summary>
    public string Id { get; private set; }
    /// <summary>
    /// 装备名称
    /// </summary>
    public string Name { get; private set; }
    /// <summary>
    /// 装备描述
    /// </summary>
    public string Desc { get; private set; }
    /// <summary>
    /// 图片名称
    /// </summary>
    public string Icon { get; private set; }
    /// <summary>
    /// 位置
    /// </summary>
    public string Weizhi { get; private set; }
    /// <summary>
    /// 武器类型
    /// </summary>
    public string WeaponType { get; private set; }
    /// <summary>
    /// 血
    /// </summary>
    public int HP { get; private set; }
    /// <summary>
    /// 攻击
    /// </summary>
    public int Att { get; private set; }
    /// <summary>
    /// 移速（攻速）
    /// </summary>
    public int Speed { get; private set; }
    /// <summary>
    /// 护甲
    /// </summary>
    public int Armor { get; private set; }
    /// <summary>
    /// 法术抗性
    /// </summary>
    public int ArmorMagic { get; private set; }
    /// <summary>
    /// 破甲（固定值）
    /// </summary>
    public int PassAtt { get; private set; }
    /// <summary>
    /// 破魔（百分比）
    /// </summary>
    public int PassMagic { get; private set; }
    /// <summary>
    /// 技能cd（百分比）
    /// </summary>
    public int SkillCD { get; private set; }
    /// <summary>
    /// 暴击率（百分比）
    /// </summary>
    public int Baoji { get; private set; }
    /// <summary>
    /// 爆伤（百分比）
    /// </summary>
    public int BaojiShanghai { get; private set; }
    /// <summary>
    /// 闪避率（百分比）
    /// </summary>
    public int Shanbi { get; private set; }
    /// <summary>
    /// 命中（百分比）
    /// </summary>
    public int Mingzhong { get; private set; }
    /// <summary>
    /// 体重
    /// </summary>
    public int Tizhong { get; private set; }
    /// <summary>
    /// 远程攻击cd
    /// </summary>
    public int RemoteAttCD { get; private set; }

    public const int __ID__ = 1123362062;
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
        + "Icon:" + Icon + ","
        + "Weizhi:" + Weizhi + ","
        + "WeaponType:" + WeaponType + ","
        + "HP:" + HP + ","
        + "Att:" + Att + ","
        + "Speed:" + Speed + ","
        + "Armor:" + Armor + ","
        + "ArmorMagic:" + ArmorMagic + ","
        + "PassAtt:" + PassAtt + ","
        + "PassMagic:" + PassMagic + ","
        + "SkillCD:" + SkillCD + ","
        + "Baoji:" + Baoji + ","
        + "BaojiShanghai:" + BaojiShanghai + ","
        + "Shanbi:" + Shanbi + ","
        + "Mingzhong:" + Mingzhong + ","
        + "Tizhong:" + Tizhong + ","
        + "RemoteAttCD:" + RemoteAttCD + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
