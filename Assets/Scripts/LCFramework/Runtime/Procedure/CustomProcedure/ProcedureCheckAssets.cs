using cfg;
using Native.Component;
using Native.Event;
using Native.FSM;
using System.Diagnostics;

namespace Native.Procedure
{
    public class ProcedureCheckAssets : ProcedureBase
    {
        public bool _copyAssetDone;

        //config
        private const string _jsonPath = "Assets/AssetBundleRes/Main/Config/{0}.json";
        private bool _startLoadConfig;

        private int _needLoadConfigCount;
        private int _loadConfigCount;

        public override void OnInit(IFSM fsm)
        {
            base.OnInit(fsm);
        }

        public override void OnEnter(IFSM fsm)
        {
            base.OnEnter(fsm);
            LaunchComponent.Event.Subscribe(CopyAssetToPersistenceDone.EventId, OnCopyAssetToPersistenceDone);
            _copyAssetDone = LaunchComponent.Resource.CheckAssetsNeedCopyToPersistencePath();
        }

        public override void OnUpdate(IFSM fsm, float elpaseSecond)
        {
            base.OnUpdate(fsm, elpaseSecond);

            if (!_copyAssetDone) return;

            if (!_startLoadConfig)
            {
                _startLoadConfig = true;
                StartLoadConfig();
                return;
            }

            if (_loadConfigCount != _needLoadConfigCount) return;
            Exl.Init();
            ConfigManager.Instance.LoadConfigDone = true;

            fsm.ChangeState<ProcedureInitManager>();
        }

        public override void OnExit(IFSM fsm)
        {
            base.OnExit(fsm);
            LaunchComponent.Event.UnSubscribe(CopyAssetToPersistenceDone.EventId, OnCopyAssetToPersistenceDone);
        }

        private void OnCopyAssetToPersistenceDone(object sender, EventArgsBase e)
        {
            _copyAssetDone = true;
        }

        private void StartLoadConfig()
        {
            var properies = typeof(Tables).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            _needLoadConfigCount = properies.Length;
            for (int i = 0; i < properies.Length; i++)
            {
                var propertyName = properies[i].Name;
                var fileName = propertyName.Replace("Tb", string.Empty).ToLower() + '_' + propertyName.ToLower();
                LaunchComponent.Resource.LoadAsset(string.Format(_jsonPath, fileName), new Resource.LoadAssetCallback(
                    (name, asset, time, userData) =>
                    {
                        Exl._loadConfigJson.Add(fileName, asset.ToString());
                        _loadConfigCount++;
                    }));
            }
        }
    }
}
