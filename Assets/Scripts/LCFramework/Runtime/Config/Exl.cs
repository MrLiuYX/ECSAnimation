using cfg;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Native.Event;
using Native.Component;
/// <summary>
/// excel工具类
/// </summary>
public class Exl
{
    //原版Luban加载方式
    //private const string jsonPath = "/../GenerateDatas/json/";
    private const string jsonPath = "Assets/AssetBundleRes/Main/Config/{0}.json";

    public static Dictionary<string, string> _loadConfigJson { set; get; }

    static Exl()
    {
        _loadConfigJson = new Dictionary<string, string>();
    }

    /// <summary>
    /// 所有excel数据
    /// </summary>
    public static Tables TB { set; get; }

    public static void Init()
    {
        TB = new Tables(Laodjson);
    }

    public static JSONNode Laodjson(string fileName)
    {
        return JSON.Parse(_loadConfigJson[fileName]);
    }
}
