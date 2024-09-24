using System.Runtime.InteropServices;

namespace YT_DLP_Web_App_Backend.Helpers
{
    public static class DependenciesHelper
    {
        const string DefaultYtDlpCommand = "yt-dlp";
        const string DefaultFfmpegCommand = "ffmpeg";

        private static bool UseLocalExecutables { get; set; } = false;
        public static string YtDlpPath { get; private set; } = DefaultYtDlpCommand;
        public static string FfmpegPath { get; private set; } = DefaultFfmpegCommand;

        public static bool DepsPresentInExeDir()
        {
            YtDlpPath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, DefaultYtDlpCommand);
            FfmpegPath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, DefaultFfmpegCommand);

            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                YtDlpPath += ".exe";
                FfmpegPath += ".exe";
            }

            if(!File.Exists(YtDlpPath))
            {
                return false;
            }

            if(!File.Exists(FfmpegPath))
            {
                return false;
            }

            return true;
        }

        public static bool DepsPresentOnPath()
        {
            if(!IsOnPath(DefaultYtDlpCommand, out string ytDlpFoundPath))
            {
                return false;
            }

            if(!IsOnPath(DefaultFfmpegCommand, out string ffmpegFoundPath))
            {
                return false;
            }


            YtDlpPath = Path.Join(ytDlpFoundPath, DefaultYtDlpCommand);
            FfmpegPath = Path.Join(ffmpegFoundPath, DefaultFfmpegCommand);

            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                YtDlpPath += ".exe";
                FfmpegPath += ".exe";
            }

            return true;
        }

        public async static Task<bool> VerifyOrInstallDependenciesAsync()
        {
            var depsPresentLocally = DepsPresentInExeDir();
            var depsPresentOnPath = DepsPresentOnPath();

            if(depsPresentLocally)
            {
                return true;
            }

            if(depsPresentOnPath)
            {
                return true;
            }

            await YoutubeDLSharp.Utils.DownloadYtDlp(Path.GetDirectoryName(YtDlpPath));
            await YoutubeDLSharp.Utils.DownloadFFmpeg(Path.GetDirectoryName(FfmpegPath));

            UseLocalExecutables = true;
            return true;
        }

        private static bool IsOnPath(string exeName, out string fullPath)
        {
            fullPath = String.Empty;
            string? paths = Environment.GetEnvironmentVariable("PATH");

            if(string.IsNullOrEmpty(paths))
            {
                return false;
            }
                

            char pathSeparator = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ';' : ':';

            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !exeName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            {
                exeName += ".exe";
            }

            var matchingPath = paths.Split(pathSeparator).FirstOrDefault(path => File.Exists(Path.Combine(path, exeName)));

            if(matchingPath == null)
            {
                return false;
            }

            fullPath = matchingPath;
            return true;
        }
    }
}
