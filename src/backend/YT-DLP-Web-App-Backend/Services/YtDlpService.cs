using YT_DLP_Web_App_Backend.Database.Entities;
using YT_DLP_Web_App_Backend.DataObjects;
using YoutubeDLSharp;
using YT_DLP_Web_App_Backend.Constants;
using YoutubeDLSharp.Metadata;
using YoutubeDLSharp.Options;
using YT_DLP_Web_App_Backend.Helpers;
using YT_DLP_Web_App_Backend.Database;
using Microsoft.EntityFrameworkCore;

namespace YT_DLP_Web_App_Backend.Services
{
    public class YtDlpService(VideoDbContext videoDbContext)
    {
        public async Task<VideoDimensions> GetMaxVideoResolutionAsync(string url)
        {
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                throw new Exception($"Invalid url {url}");
            }

            string stdOutput = "";
            YoutubeDLProcess ytdlProc = new(DependenciesHelper.YtDlpPath);
            ytdlProc.OutputReceived += (_, e) => { stdOutput += e.Data + "\n"; };

            OptionSet options = new()
            {
                Quiet = true,
                NoWarnings = true,
                SkipDownload = true,
            };
            options.AddCustomOption<string>("--print", "width");
            options.AddCustomOption<string>("--print", "height");

            string[] urls = [url];
            int success = await ytdlProc.RunAsync(urls, options);

            if (success > 0)
            {
                throw new Exception($"Failed to fetch video dimensions.");
            }

            string[] lines = stdOutput.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length < 2)
            {
                return new(0, 0);
            }

            return new(int.Parse(lines[0]), int.Parse(lines[1]));
        }

        public async Task<VideoData> GetVideoDataAsync(string url)
        {
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                throw new Exception($"Invalid url {url}");
            }

            YoutubeDL ytdl = new()
            {
                YoutubeDLPath = DependenciesHelper.YtDlpPath,
                FFmpegPath = DependenciesHelper.FfmpegPath,
                OutputFolder = AppConstants.DefaultDownloadDir
            };

            RunResult<VideoData> runResult = await ytdl.RunVideoDataFetch(url);

            if (!runResult.Success)
            {
                string errorString = string.Join("\n", runResult.ErrorOutput);
                throw new Exception($"YtDlp failed with error: {errorString}");
            }

            return runResult.Data;
        }

        public async Task DownloadVideoAsync(string url, string videoName, int videoId, VideoDimensions? dimensions,
            CancellationToken cancellationToken = default)
        {
            Video videoRecord = await videoDbContext.Videos.FirstOrDefaultAsync(x => x.Id == videoId) ??
                                 throw new Exception(
                                     "No existing video record found when attempting to download the video.");
            try
            {
                if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    throw new Exception($"Invalid url {url}");
                }

                MediaInProgressStorage.AddVideoToTrack(videoRecord.Id);

                YoutubeDL ytdl = new()
                {
                    YoutubeDLPath = DependenciesHelper.YtDlpPath,
                    FFmpegPath = DependenciesHelper.FfmpegPath,
                    OutputFolder = AppConstants.DefaultDownloadDir
                };

                string requestedFormat;
                if (dimensions != null && dimensions.Height > 0 && dimensions.Width > 0)
                {
                    requestedFormat = $"bestvideo[height={dimensions.Height}][width={dimensions.Width}]+bestaudio/best";
                }
                else
                {
                    requestedFormat = "bestvideo+bestaudio/best";
                }

                Progress<DownloadProgress> progress = new(x =>
                {
                    MediaInProgressStorage.UpdateVideoPercent(videoRecord.Id, 100 * x.Progress);
                });

                OptionSet options = new()
                {
                    Output = Path.Join(AppConstants.DefaultDownloadDir, videoName + ".%(ext)s"),
                    WriteThumbnail = true,
                    MergeOutputFormat = DownloadMergeFormat.Mp4,
                    Format = requestedFormat,
                    RecodeVideo = VideoRecodeFormat.Mp4,
                    ConvertThumbnails = "jpg",
                };

                RunResult<string> result = await ytdl.RunVideoDownload(url, progress: progress, ct: cancellationToken,
                    overrideOptions: options);

                if (!result.Success)
                {
                    var errorString = string.Join("\n", result.ErrorOutput);
                    throw new Exception(errorString);
                }

                string finalFilePath = Path.Join(AppConstants.DefaultDownloadDir, videoRecord.FileName);
                videoRecord.Downloaded = true;
                videoRecord.ThumbnailName = videoName + ".jpg";
                videoRecord.Size = new FileInfo(finalFilePath).Length;
                videoRecord.UpdatedAt = DateTime.UtcNow;

                await videoDbContext.SaveChangesAsync();
                MediaInProgressStorage.MarkVideoDownloaded(videoRecord.Id);
            }
            catch (Exception ex)
            {
                string finalFilePath = Path.Join(AppConstants.DefaultDownloadDir, videoRecord.FileName);
                string finalThumbnailPath = Path.Join(AppConstants.DefaultDownloadDir, videoName + ".jpg");

                if (File.Exists(finalFilePath))
                {
                    File.Delete(finalFilePath);
                }

                if (File.Exists(finalThumbnailPath))
                {
                    File.Delete(finalThumbnailPath);
                }

                videoDbContext.Remove(videoRecord);
                await videoDbContext.SaveChangesAsync();

                throw new Exception($"Failed to download video due to ex: {ex}");
            }
        }
        
        public async Task DownloadSavedVideo(int videoId)
        {
            Video? video = await videoDbContext.Videos.FirstOrDefaultAsync(x => x.Id == videoId);

            if(video == null)
            {
                return;
            }

            string videoPath = Path.Join(AppConstants.DefaultDownloadDir, video.FileName);
            if(File.Exists(videoPath))
            {
                File.Delete(videoPath);
            }

            string thumbnailPath = Path.Join(AppConstants.DefaultDownloadDir, video.FileName);
            if(!string.IsNullOrEmpty(video.ThumbnailName) && File.Exists(thumbnailPath))
            {
                File.Delete(thumbnailPath);
            }

            string mp3Path = Path.Join(AppConstants.DefaultDownloadDir, video.Mp3FileName);
            if(!string.IsNullOrEmpty(video.Mp3FileName) && File.Exists(mp3Path))
            {
                File.Delete(mp3Path);
            }

            await DownloadVideoAsync(video.Url, video.FileName.TrimEnd(Path.GetExtension(video.FileName)), video.Id, null);
        }
    }
}