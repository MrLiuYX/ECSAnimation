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



namespace cfg.entityAttributeConfig
{ 

public sealed partial class EntityAttributeConfig :  Bright.Config.BeanBase 
{
    public EntityAttributeConfig(JSONNode _json) 
    {
        { if(!_json["Id"].IsNumber) { throw new SerializationException(); }  Id = _json["Id"]; }
        { var __json0 = _json["Attributes"]; if(!__json0.IsArray) { throw new SerializationException(); } Attributes = new System.Collections.Generic.List<AttributeType>(__json0.Count); foreach(JSONNode __e0 in __json0.Children) { AttributeType __v0;  { if(!__e0.IsNumber) { throw new SerializationException(); }  __v0 = (AttributeType)__e0.AsInt; }  Attributes.Add(__v0); }   }
        { var __json0 = _json["Values"]; if(!__json0.IsArray) { throw new SerializationException(); } Values = new System.Collections.Generic.List<float>(__json0.Count); foreach(JSONNode __e0 in __json0.Children) { float __v0;  { if(!__e0.IsNumber) { throw new SerializationException(); }  __v0 = __e0; }  Values.Add(__v0); }   }
        PostInit();
    }

    public EntityAttributeConfig(long Id, System.Collections.Generic.List<AttributeType> Attributes, System.Collections.Generic.List<float> Values ) 
    {
        this.Id = Id;
        this.Attributes = Attributes;
        this.Values = Values;
        PostInit();
    }

    public static EntityAttributeConfig DeserializeEntityAttributeConfig(JSONNode _json)
    {
        return new entityAttributeConfig.EntityAttributeConfig(_json);
    }

    /// <summary>
    /// 实体Id
    /// </summary>
    public long Id { get; private set; }
    /// <summary>
    /// 属性
    /// </summary>
    public System.Collections.Generic.List<AttributeType> Attributes { get; private set; }
    /// <summary>
    /// 属性值
    /// </summary>
    public System.Collections.Generic.List<float> Values { get; private set; }

    public const int __ID__ = -1651118232;
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
        + "Attributes:" + Bright.Common.StringUtil.CollectionToString(Attributes) + ","
        + "Values:" + Bright.Common.StringUtil.CollectionToString(Values) + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}