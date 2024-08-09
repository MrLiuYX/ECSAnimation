using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using Native.Component;
using System;

public class GameManagerDataBase<T> where T : class, IGameManagerData, new()
{
    public T Data;

    private Action _resetData;

    private void ResetData()
    {
        if(_resetData == null)
        {
            var methodReset = typeof(T).GetMethod("ResetStaticData", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public |  System.Reflection.BindingFlags.NonPublic);
            if (methodReset == null) return;
            _resetData = () => { methodReset.Invoke(Data, null); };
        }

        _resetData.Invoke();
    }

    private void GetDataFormSetting()
    {
        Data = JsonMapper.ToObject<T>(LaunchComponent.Setting.GetString(typeof(T).Name, string.Empty));        
        if(Data == null)
        {
            Data = new T();
        }
    }

    private void SaveDataToSetting()
    {
        LaunchComponent.Setting.SetString(typeof(T).Name,JsonMapper.ToJson(Data));
        LaunchComponent.Setting.SaveSync(false);
    }
}