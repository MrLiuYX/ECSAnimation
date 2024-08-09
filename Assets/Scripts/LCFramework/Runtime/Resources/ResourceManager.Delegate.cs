namespace Native.Resource
{
    public delegate void LoadAssetSuccessCallback(string name, object asset, float time, object userData);
    public delegate void LoadAssetFailCallback(string name, float time, object userData);
    public delegate void LoadBinarySuccessCallback(string name, byte[] bytes, float time, object userData);
    public delegate void LoadBinaryFailCallback(string name, float time, object userData);
}