using Native.Component;
using Native.Event;
using Native.Resource;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using Native.Construct;

namespace Native.Setting
{
    public class SettingManager : ManagerBase<SettingManager>, ISettingManager
    {
        private string _saveDataPath;
        private Dictionary<string, string> _allValue;
        private Vector2 _editorScrollerPos;
        private string _editorFindKey;
        private bool _editorFindKeyIgnorCase;

        public Dictionary<string, string> AllValue { get { return _allValue; } }

        public override void Init()
        {
            base.Init();
            _allValue = new Dictionary<string, string>();
            _saveDataPath = Path.Combine(LaunchComponent.Resource.PersistenPath, "SaveGameData.setting");
        }
        public override void OnDisable()
        {
            base.OnDisable();
            SaveSync();
        }

        public void SetBool(string key, bool value)
        {
            if (!_allValue.ContainsKey(key))
            {
                _allValue.Add(key, value.ToString());
            }
            _allValue[key] = value.ToString();
        }

        public void SetFloat(string key, float value)
        {
            if (!_allValue.ContainsKey(key))
            {
                _allValue.Add(key, value.ToString());
            }
            _allValue[key] = value.ToString();
        }

        public void SetInt(string key, int value)
        {
            if (!_allValue.ContainsKey(key))
            {
                _allValue.Add(key, value.ToString());
            }
            _allValue[key] = value.ToString();
        }

        public void SetString(string key, string value)
        {
            if (!_allValue.ContainsKey(key))
            {
                _allValue.Add(key, value);
            }
            _allValue[key] = value;
        }

        public bool GetBool(string key, bool defaultValue)
        {
            if (_allValue.TryGetValue(key, out var value))
            {
                return bool.Parse(value);
            }

            SetBool(key, defaultValue);
            return defaultValue;
        }

        public float GetFloat(string key, float defaultValue)
        {
            if (_allValue.TryGetValue(key, out var value))
            {
                return float.Parse(value);
            }

            SetFloat(key, defaultValue);
            return defaultValue;
        }

        public int GetInt(string key, int defaultValue)
        {
            if (_allValue.TryGetValue(key, out var value))
            {
                return int.Parse(value);
            }

            SetInt(key, defaultValue);
            return defaultValue;
        }

        public string GetString(string key, string defaultValue)
        {
            if (_allValue.TryGetValue(key, out var value))
            {
                return value;
            }

            SetString(key, defaultValue);
            return defaultValue;
        }

        public void ReadSync()
        {
            if (!File.Exists(_saveDataPath))
            {
                UnityEngine.Debug.Log($"Read Setting done");
                LaunchComponent.Event.Fire(OnReadSettingDone.EventId, OnReadSettingDone.Create());
                return;
            }

            _allValue.Clear();
            using (FileStream fs = new FileStream(_saveDataPath, FileMode.OpenOrCreate))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    var count = br.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        _allValue.Add(br.ReadString(), br.ReadString());
                    }
                }
            }

            UnityEngine.Debug.Log($"Read Setting done");
            LaunchComponent.Event.Fire(OnReadSettingDone.EventId, OnReadSettingDone.Create());
        }

        public void SaveSync(bool fireEvent = false)
        {
            using (FileStream fs = new FileStream(_saveDataPath, FileMode.OpenOrCreate))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(_allValue.Count);
                    foreach (var item in _allValue)
                    {
                        bw.Write(item.Key);
                        bw.Write(item.Value);
                    }
                }
            }
            UnityEngine.Debug.Log($"Save Setting done");
            if(fireEvent) LaunchComponent.Event.Fire(OnSaveSettingDone.EventId, OnSaveSettingDone.Create());
        }

        public override void SubInspectorDraw()
        {
            base.SubInspectorDraw();
            GUILayout.BeginVertical("Box");
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Find key", GUILayout.Width(100));
            _editorFindKey = GUILayout.TextField(_editorFindKey);
            GUILayout.EndHorizontal();
            _editorFindKeyIgnorCase = GUILayout.Toggle(_editorFindKeyIgnorCase, "Ignor case");
            _editorScrollerPos = GUILayout.BeginScrollView(_editorScrollerPos);
            {
                foreach (var item in _allValue.Keys)
                {
                    var compareName = _editorFindKeyIgnorCase ? item.ToLower() : item;
                    var compareSrcName = _editorFindKeyIgnorCase ? _editorFindKey.ToLower() : _editorFindKey;
                    if (compareName == compareSrcName
                        || string.IsNullOrEmpty(_editorFindKey))
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label($"{item}", GUILayout.Width(100));
                        GUILayout.Label($"{_allValue[item]}");
                        GUILayout.EndHorizontal();
                    }
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
    }
}
