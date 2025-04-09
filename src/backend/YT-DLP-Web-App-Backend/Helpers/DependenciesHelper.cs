using System.Runtime.InteropServices;

namespace YT_DLP_Web_App_Backend.Helpers
{
    public static class DependenciesHelper
    {
        const string DefaultYtDlpCommand = "yt-dlp";
        const string DefaultFfmpegCommand = "ffmpeg";
        
        public static string YtDlpPath { get; private set; } = Path.Join(AppDomain.CurrentDomain.BaseDirectory, DefaultYtDlpCommand) + GetOsExtension();
        public static string FfmpegPath { get; private set; } = Path.Join(AppDomain.CurrentDomain.BaseDirectory, DefaultFfmpegCommand) + GetOsExtension();
        
        public static string GetOsExtension()
        {
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return ".exe";
            }
            else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "";
            }
            else if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "";
            }

            throw new NotSupportedException("Unsupported OS");
        }

        public static bool ValidateDependencies()
        {
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

        public static async Task<bool> VerifyOrInstallDependenciesAsync()
        {
            var depsPresentLocally = ValidateDependencies();

            if(depsPresentLocally)
            {
                return true;
            }

            await YoutubeDLSharp.Utils.DownloadYtDlp(Path.GetDirectoryName(YtDlpPath));
            await YoutubeDLSharp.Utils.DownloadFFmpeg(Path.GetDirectoryName(FfmpegPath));
            
            return true;
        }
        
        public static async Task<bool> UpdateYtDlpAsync()
        {
            var result = false;
            var newDownloadDirectory = Path.Join(Path.GetDirectoryName(YtDlpPath), "new-download");
            
            if(!Directory.Exists(newDownloadDirectory))
            {
                Directory.CreateDirectory(newDownloadDirectory);
            }
            
            await YoutubeDLSharp.Utils.DownloadYtDlp(newDownloadDirectory);
            var newPath = FindYtDlpInPath(newDownloadDirectory);
            
            try
            {
                File.Delete(YtDlpPath);
                File.Move(newPath, YtDlpPath);
                result = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to update yt-dlp: " + e);
            }
            
            if (File.Exists(newPath))
            {
                File.Delete(newPath);
                result = false;
            }
            
            if(Directory.Exists(newDownloadDirectory))
            {
                Directory.Delete(newDownloadDirectory, true);
            }

            return result;
        }

        private static string FindYtDlpInPath(string path)
        {
            var pathFiles = Directory.GetFiles(path);
            var ytDlpFiles = pathFiles.Where(x => x.Contains("yt-dlp")).ToList();

            return ytDlpFiles.FirstOrDefault() ?? string.Empty;
        }
    }
}
