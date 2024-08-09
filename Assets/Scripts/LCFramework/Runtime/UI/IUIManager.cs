using UnityEngine;

namespace Native.UI
{
    public interface IUIManager
    {
        public Camera UICamera { get; }
        public Canvas UICanvas { get; }
        public Transform UIRoot { get; }
        public float UIFormDestroyTime { get; }
        public int OpenUIForm(string path, string groupName, bool allowMultiplyInstance, object userData);
        public void CloseUIForm(int serializeID, object userData = null);
        public void CreateUIGroup(string[] groupNames, int[] groupDeepLayers);
        public IUIGroup GetUIGroup(string groupName);
        public IUIForm GetUIForm(int serializeId);
        public void SetGroupData(string[] groupNames, int[] groupDeepLayers);
    }
}
