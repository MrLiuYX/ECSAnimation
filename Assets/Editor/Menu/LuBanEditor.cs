using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;
using System.IO;
using System.Data;
using LitJson;
using System.Collections.Generic;

public class LuBanEditor
{
    [MenuItem("LCFramework/Luban")]
    public static async void Run()
    {
        Directory.Delete("Assets/AssetBundleRes/Main/Config", true);

        AssetDatabase.Refresh();
        var process = new Process();
        process.StartInfo.FileName = @$"{Application.dataPath.Replace("Assets", string.Empty)}/luban/gen_code_json.bat"; //指定要执行的批处理文件
        process.StartInfo.WorkingDirectory = @$"{Application.dataPath.Replace("Assets", string.Empty)}/luban";
        process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
        process.Start();
        while (!File.Exists("Assets/AssetBundleRes/Main/Config/uiformconfig_tbuiformconfig.json"))
            await Task.Delay(100);
        ReadExcel();
        while (!File.Exists("Assets/AssetBundleRes/Main/Config/soliderattributeconfig_tbsoliderattributeconfig.json"))
            await Task.Delay(100);
        ReadAttribute();
    }

    private static void ReadExcel()
    {
        var data = File.ReadAllText("Assets/AssetBundleRes/Main/Config/uiformconfig_tbuiformconfig.json");
        var uiForms = JsonMapper.ToObject(data);
        Queue<int> ids = new Queue<int>();
        Queue<string> names = new Queue<string>();
        for (int i = 0; i < uiForms.Count; i++)
        {
            ids.Enqueue(int.Parse(uiForms[i]["Id"].ToString()));
            names.Enqueue(uiForms[i]["AssetName"].ToString());
        }

        var filePath = "Assets/Scripts/LCFramework/Runtime/UI/UIFormId.cs";
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        using (FileStream fs = new FileStream(filePath, FileMode.Create))
        {
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine("public enum UIFormId");
                sw.WriteLine("{");
                while (names.Count != 0)
                {
                    sw.WriteLine($"\t{names.Dequeue()} = {ids.Dequeue()},");
                }
                sw.WriteLine("}");
            }
        }
        AssetDatabase.Refresh();
    }
    private static void ReadAttribute()
    {
        var data = File.ReadAllText("Assets/AssetBundleRes/Main/Config/soliderattributeconfig_tbsoliderattributeconfig.json");
        var uiForms = JsonMapper.ToObject(data);
        Queue<string> names = new Queue<string>();
        Queue<string> ids = new Queue<string>();
        for (int i = 0; i < uiForms.Count; i++)
        {
            names.Enqueue(uiForms[i]["EnmuName"].ToString());
            ids.Enqueue(uiForms[i]["SoliderID"].ToString());
        }

        var filePath = "Assets/Scripts/LCFramework/Runtime/Custom/Enum/SoliderIdEnum.cs";
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        using (FileStream fs = new FileStream(filePath, FileMode.Create))
        {
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine("public enum SoliderIdEnum");
                sw.WriteLine("{");
                while (names.Count != 0)
                {
                    sw.WriteLine($"\t{names.Dequeue()} = {ids.Dequeue()},");
                }
                sw.WriteLine("}");
            }
        }
        AssetDatabase.Refresh();
    }
}
