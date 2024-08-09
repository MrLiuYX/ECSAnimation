using UnityEngine;
using Native.UI;
using Native.Component;
using Native.Audio;
using cfg.audioConfig;
using System.Collections.Generic;

namespace Native
{
    public static class ComponentExtension
    {
        private const string UIFormPath = "Assets/AssetBundleRes/Main/Prefabs/UIForms/{0}/{1}.prefab";
        private static Dictionary<UIFormId, int> _uis = new Dictionary<UIFormId, int>();

        public static T AddOrGetComponent<T>(this UnityEngine.Component component) where T : UnityEngine.Component
        {            
            var returnComponent = component.GetComponent<T>();
            returnComponent = returnComponent == null ? component.gameObject.AddComponent<T>() : returnComponent;
            return returnComponent;
        }

        public static int OpenUIForm(this IUIManager manager, UIFormId id, object userData = null)
        {
            var info = Exl.TB.TbUIFormConfig.Get(id.ToInt());
            var uiId = LaunchComponent.UI.OpenUIForm(string.Format(UIFormPath, info.UIGroupName, info.AssetName), info.UIGroupName, info.AllowMultiInstance, userData);
            if (!info.AllowMultiInstance
                && !_uis.ContainsKey(id))
                _uis.Add(id, uiId);
            return uiId;
        }

        public static T GetUIForm<T>(this IUIManager manager, UIFormId id) where T : UIFormLogic
        {
            _uis.TryGetValue(id, out var ui);
            return manager.GetUIForm(ui)?.UIFormLogic as T;
        }

        private static Dictionary<int, AudioConfig> _audioConfigs;

        public static int PlayAudio(this IAudioManager manager, int id, bool loop = false)
        {
            if (_audioConfigs == null)
                _audioConfigs = Exl.TB.TbAudioConfig.DataMap;
            var currentCfg = _audioConfigs[id];
            return manager.PlayAudio(string.Format(AudioManager.AudioPath, currentCfg.AudioGroup, currentCfg.Id), currentCfg.AudioGroup, AudioData.Create2D(loop, 1));
        }
    }
}

