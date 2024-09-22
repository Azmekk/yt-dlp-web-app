namespace YT_DLP_Web_App_Backend.Constants
{
    public static class AppConstants
    {
        public static readonly string SqliteFolderPath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "Database");
        public static readonly string SqliteFilePath = Path.Join(SqliteFolderPath, "app_database.db");

        public static readonly string DefaultDownloadDir = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "Downloads");
    }
}
