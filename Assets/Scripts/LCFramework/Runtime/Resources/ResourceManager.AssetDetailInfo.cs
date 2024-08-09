namespace Native.Resource
{
    public class AssetInfo
    {
        public enum AssetModeEnum
        {
            Normal = 1,
            Binary = 2,
        }

        public int AssetMode;
        public string AssetPath;
        public string ResoucePath;

        public AssetModeEnum GetAssetMode()
        {
            return (AssetModeEnum)AssetMode;
        }
    }

    public class ResourceInfo
    {
        public string ResoucePath;
        public string[] DependResourcePath;
    }
}

