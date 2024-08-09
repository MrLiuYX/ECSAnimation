using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Native.Component;
using Native.Event;

namespace Native
{
    public enum LanguageType
    {
        CS,
        EN
    }

    public partial class ConfigManager
    {
        private LanguageType _languageType;
        public LanguageType CurrentLanguageType
        {
            get
            {
                return _languageType;
            }

            set
            {
                if (_languageType != value)
                    LaunchComponent.Event.Fire(OnLanguageChangedEvent.EventId, OnLanguageChangedEvent.Create());
                _languageType = value;
            }
        }

        private Dictionary<Text, string> _reigsterComponents = new Dictionary<Text, string>();
        private List<Text> _needRemoveComponents = new List<Text>();
        private Dictionary<(LanguageType, string), string> _allMultiplyLanguageValues = new Dictionary<(LanguageType, string), string>();
        private const string NoneKey = "None Key";
        private bool _subEvent = false;

        public string GetKey(string key)
        {
            return InternalGetKey(key);
        }

        public void ReigsterLanguage(Text component)
        {
            var key = component.text;
            var value = InternalGetKey(key);
            if (value == NoneKey)
            {
                return;
            }

            component.text = value;
            _reigsterComponents.Add(component, key);

            if (!_subEvent)
            {
                _subEvent = true;
                LaunchComponent.Event.Subscribe(OnLanguageChangedEvent.EventId, OnLanguageChanged);
            }
        }

        public void RemoveLanguage(Text component)
        {
            if (_reigsterComponents.ContainsKey(component))
                _reigsterComponents.Remove(component);
        }

        public static void ReigsterLanguage(Text component, string key)
        {
            var value = Instance.InternalGetKey(key);
            if (value == NoneKey)
            {
                return;
            }

            component.text = value;
            Instance._reigsterComponents.Add(component, key);

            if (!Instance._subEvent)
            {
                Instance._subEvent = true;
                LaunchComponent.Event.Subscribe(OnLanguageChangedEvent.EventId, Instance.OnLanguageChanged);
            }
        }

        private string InternalGetKey(string key)
        {
            if (_allMultiplyLanguageValues.Count == 0)
            {
                Init();
            }

            if (_allMultiplyLanguageValues.TryGetValue((CurrentLanguageType, key), out var value))
            {
                return value;
            }

            return NoneKey;
        }

        private void Init()
        {
            var lists = Exl.TB.TbTranslate.DataList;
            for (int i = 0; i < lists.Count; i++)
            {
                var info = lists[i];
                _allMultiplyLanguageValues.Add((LanguageType.CS, info.Id), info.ZhCN);
                _allMultiplyLanguageValues.Add((LanguageType.EN, info.Id), info.EnEU);
            }
        }

        private void OnLanguageChanged(object sender, IEventArgs e)
        {

            foreach (var item in _reigsterComponents)
            {
                if (item.Key == null)
                {
                    _needRemoveComponents.Add(item.Key);
                    continue;
                }

                item.Key.text = InternalGetKey(item.Value);
            }

            for (int i = 0; i < _needRemoveComponents.Count; i++)
            {
                _reigsterComponents.Remove(_needRemoveComponents[i]);
            }
            _needRemoveComponents.Clear();
        }
    }
}
