using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Native.Component;
using Native.Event;
using Native.Procedure;

public class GameManagerBase<Class, Dat> : IGameManager where Class : class, IGameManager, new () where Dat : class, IGameManagerData, new ()
{
    public Dat Data => _dataClass.Data;

    private static Class _instance;
    private GameManagerDataBase<Dat> _dataClass;
    private Action _getDataFormSetting;
    private Action _saveDataToSetting;
    private Action _resetData;

    public static Class Instance
    {
        get 
        {
            if(_instance == null)
            {                
                _instance = new Class();
                _instance.InitData();
            }
            return _instance;
        }
    }


    public void InitData()
    {
        var instanceType = typeof(GameManagerDataBase<>).MakeGenericType(typeof(Dat));
        var instance = Activator.CreateInstance(instanceType) as GameManagerDataBase<Dat>;
        _dataClass = instance;

        var methodGet = instanceType.GetMethod("GetDataFormSetting", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
        var methodSave = instanceType.GetMethod("SaveDataToSetting", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
        var methodReset = instanceType.GetMethod("ResetData", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

        _getDataFormSetting = ()=> { methodGet?.Invoke(_dataClass, null); };
        _saveDataToSetting = () => { methodSave?.Invoke(_dataClass, null); };
        _resetData = () => { methodReset?.Invoke(_dataClass, null); };

        _getDataFormSetting.Invoke();
    }

    public void ResetData()
    {
        _resetData.Invoke();
    }

    public void SaveData()
    {
        _saveDataToSetting.Invoke();
    }

    public virtual void InitManager()
    {
        LaunchComponent.Event.Subscribe(OnProcedureChangedEvent.EventId, OnProcedureChanged);
    }

    public virtual void OnUpdate(float elapseSeconds)
    {

    }

    protected Type CurrentProcedure;
    protected Type LastProcedure;
    public virtual void OnProcedureChanged(object sender, IEventArgs e)
    {
        CurrentProcedure = LaunchComponent.Procedure.GetCurrentProcedure().GetType();
        LastProcedure = ((OnProcedureChangedEvent)e).LastProcedure;
    }

    public virtual void OnEnterGame()
    {
        
    }

    public virtual void OnExitGame()
    {
        
    }
}
