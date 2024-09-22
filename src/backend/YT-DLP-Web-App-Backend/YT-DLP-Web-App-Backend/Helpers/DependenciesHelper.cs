using System.Runtime.InteropServices;

namespace YT_DLP_Web_App_Backend.Helpers
{
    public static class DependenciesHelper
    {
        const string DefaultYtDlpCommand = "yt-dlp";
        const string DefaultFfmpegCommand = "ffmpeg";
        const string DefaultFfProbeCommand = "ffprobe";

        private static bool UseLocalExecutables { get; set; } = false;

        static string ytDlpFullPath = "";
        static string ffmpegFullPath = "";
        static string ffprobeFullPath = "";
        public static string YtDlpPath
        {
            get
            {
                if(UseLocalExecutables)
                {
                    return ytDlpFullPath;
                }

                return DefaultYtDlpCommand;
            }
        }

        public static string FfmpegPath
        {
            get
            {
                if(UseLocalExecutables)
                {
                    return ffmpegFullPath;
                }

                return DefaultFfmpegCommand;
            }
        }

        public static string FfprobePath
        {
            get
            {
                if(UseLocalExecutables)
                {
                    return ffprobeFullPath;
                }

                return DefaultFfProbeCommand;
            }
        }

        public static bool DepsPresentInExeDir()
        {
            ytDlpFullPath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, DefaultYtDlpCommand);
            ffmpegFullPath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, DefaultFfmpegCommand);
            ffprobeFullPath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, DefaultFfProbeCommand);

            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                ytDlpFullPath += ".exe";
                ffmpegFullPath += ".exe";
                ffprobeFullPath += ".exe";
            }

            if(!File.Exists(ytDlpFullPath))
            {
                return false;
            }

            if(!File.Exists(ffmpegFullPath))
            {
                return false;
            }

            if(!File.Exists(ffprobeFullPath))
            {
                return false;
            }

            return true;
        }

        public static bool DepsPresentOnPath()
        {
            if(!IsOnPath(DefaultYtDlpCommand))
            {
                return false;
            }

            if(!IsOnPath(DefaultFfmpegCommand))
            {
                return false;
            }

            if(!IsOnPath(DefaultFfProbeCommand))
            {
                return false;
            }

            return true;
        }

        public async static Task<bool> VerifyOrInstallDependenciesAsync()
        {
            var depsPresentLocally = DepsPresentInExeDir();
            var depsPresentOnPath = DepsPresentOnPath();

            if(depsPresentLocally)
            {
                UseLocalExecutables = true;
                return true;
            }

            if(depsPresentOnPath)
            {
                UseLocalExecutables = false;
                return true;
            }

            await YoutubeDLSharp.Utils.DownloadYtDlp();
            await YoutubeDLSharp.Utils.DownloadFFmpeg();

            UseLocalExecutables = true;
            return true;
        }

        private static bool IsOnPath(string exeName)
        {
            string? paths = Environment.GetEnvironmentVariable("PATH");

            if(string.IsNullOrEmpty(paths))
                return false;

            char pathSeparator = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ';' : ':';

            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !exeName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            {
                exeName += ".exe";
            }

            return paths.Split(pathSeparator).Any(
                path => File.Exists(Path.Combine(path, exeName)));
        }
    }
}
