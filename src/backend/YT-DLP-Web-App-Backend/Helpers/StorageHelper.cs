namespace YT_DLP_Web_App_Backend.Helpers
{
    public static class StorageHelper
    {
        public static long GetSize(this DirectoryInfo dir)
        {
            return dir.GetFiles().Sum(fi => fi.Length) +
                   dir.GetDirectories().Sum(di => GetSize(di));
        }
    }
}
