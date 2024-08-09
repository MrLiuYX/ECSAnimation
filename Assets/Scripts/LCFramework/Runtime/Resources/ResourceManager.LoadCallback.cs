namespace Native.Resource
{
    public class LoadAssetCallback
    {
        public LoadAssetSuccessCallback LoadAssetSuccessCallback;
        public LoadAssetFailCallback LoadAssetFailCallback;

        public LoadAssetCallback(
            LoadAssetSuccessCallback loadAssetSuccessCallback)
        {
            LoadAssetSuccessCallback = loadAssetSuccessCallback;
        }

        public LoadAssetCallback(
            LoadAssetSuccessCallback loadAssetSuccessCallback
            , LoadAssetFailCallback loadAssetFailCallback)
        {
            LoadAssetSuccessCallback = loadAssetSuccessCallback;
            LoadAssetFailCallback = loadAssetFailCallback;
        }
    }

    public class LoadBinaryCallback
    {
        public LoadBinarySuccessCallback LoadBinarySuccessCallback;
        public LoadBinaryFailCallback LoadBinaryFailCallback;

        public LoadBinaryCallback(
            LoadBinarySuccessCallback loadBinarySuccessCallback)
        {
            LoadBinarySuccessCallback = loadBinarySuccessCallback;
        }

        public LoadBinaryCallback(
            LoadBinarySuccessCallback loadBinarySuccessCallback
            , LoadBinaryFailCallback loadBinaryFailCallback)
        {
            LoadBinarySuccessCallback = loadBinarySuccessCallback;
            LoadBinaryFailCallback = loadBinaryFailCallback;
        }
    }
}