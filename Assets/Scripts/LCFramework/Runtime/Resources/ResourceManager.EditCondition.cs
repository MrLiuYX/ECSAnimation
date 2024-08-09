#if UNITY_EDITOR
using UnityEditor;

namespace Native.Resource
{
    public partial class ResourceManager
    {
        private void EditLoadAsset(string assetPath, LoadAssetCallback loadAssetCallback, object userData)
        {
            var obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));
            if (obj == null)
            {
                UnityEngine.Debug.LogError($"Load Error! Can't find asset {assetPath}");
                loadAssetCallback.LoadAssetFailCallback?.Invoke(assetPath, 0, userData);
            }
            else
            {
                loadAssetCallback.LoadAssetSuccessCallback?.Invoke(assetPath, obj, 0, userData);
            }
        }
    }
}
#endif