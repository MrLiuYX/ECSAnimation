using System.Collections.Generic;
using UnityEngine;
using Native.Event;
using Native.FSM;
using Native.Component;
using cfg;
using Native.Resource;

namespace Native.Procedure
{
    public class ProcedureLaunch : ProcedureBase
    {
        private int _loadAssetCount;
        private int _completeLoadAssetCount;

        public override void OnInit(IFSM fsm)
        {
            base.OnInit(fsm);
        }
        public override void OnEnter(IFSM fsm)
        {
            base.OnEnter(fsm);

            _loadAssetCount = 0;
            _completeLoadAssetCount = 3;

            LaunchComponent.Event.Subscribe(LoadAssetInfoTXTDone.EventId, OnLoadAssetInfoTxtDone);
            LaunchComponent.Event.Subscribe(LoadResourceInfoTXTDone.EventId, OnLoadResourceInfoTxtDone);
            LaunchComponent.Event.Subscribe(Native.Event.OnReadSettingDone.EventId, OnReadSettingDone);
#if UNITY_EDITOR
            if ((LaunchComponent.Resource as ResourceManager).IsUseEditLoadType)
            {
                _loadAssetCount = _completeLoadAssetCount - 1;
            }
            else
            {
                LaunchComponent.Resource.UpdateAssetInfoAndResouceInfo();
            }
#else
            LaunchComponent.Resource.UpdateAssetInfoAndResouceInfo();
#endif
            LaunchComponent.Setting.ReadSync();
        }
        public override void OnUpdate(IFSM fsm, float elpaseSecond)
        {
            base.OnUpdate(fsm, elpaseSecond);

            if (_loadAssetCount != _completeLoadAssetCount) return;

            fsm.ChangeState<ProcedureCheckAssets>();
        }

        public override void OnExit(IFSM fsm)
        {
            base.OnExit(fsm);
            LaunchComponent.Event.UnSubscribe(LoadAssetInfoTXTDone.EventId, OnLoadAssetInfoTxtDone);
            LaunchComponent.Event.UnSubscribe(LoadResourceInfoTXTDone.EventId, OnLoadResourceInfoTxtDone);
        }

        private void OnLoadAssetInfoTxtDone(object sender, EventArgsBase e)
        {
            _loadAssetCount++;
        }

        private void OnLoadResourceInfoTxtDone(object sender, EventArgsBase e)
        {
            _loadAssetCount++;
        }

        private void OnReadSettingDone(object sender, EventArgsBase e)
        {
            _loadAssetCount++;
        }
    }
}
