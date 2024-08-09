using System.Collections.Generic;

namespace Native.Setting
{
    public interface ISettingManager
    {
        public Dictionary<string, string> AllValue { get; }
        public int GetInt(string key, int defaultValue);
        public float GetFloat(string key, float defaultValue);
        public string GetString(string key, string defaultValue);
        public bool GetBool(string key, bool defaultValue);
        public void SetInt(string key, int value);
        public void SetFloat(string key, float value);
        public void SetString(string key, string value);
        public void SetBool(string key, bool value);
        public void SaveSync(bool fireEvent);
        public void ReadSync();
    }
}
