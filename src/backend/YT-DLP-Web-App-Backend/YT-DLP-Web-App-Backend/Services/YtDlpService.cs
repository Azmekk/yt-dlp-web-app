using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Json;
using YT_DLP_Web_App_Backend.Database.Entities;
using YT_DLP_Web_App_Backend.DataObjects;
using YoutubeDLSharp;
using YT_DLP_Web_App_Backend.Constants;
using YoutubeDLSharp.Metadata;
using YoutubeDLSharp.Options;
using YT_DLP_Web_App_Backend.Helpers;
using Microsoft.Extensions.Options;
using YT_DLP_Web_App_Backend.Database;
using Microsoft.EntityFrameworkCore;

namespace YT_DLP_Web_App_Backend.Services
{
    public class YtDlpService(VideosService videosService, VideoDbContext videoDbContext)
    {
        public async Task<VideoDimensions> GetMaxVideoResolutionAsync(string url)
        {
            if(!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                throw new Exception($"Invalid url {url}");
            }

            string stdOutput = "";
            YoutubeDLProcess ytdlProc = new(DependenciesHelper.YtDlpPath);
            ytdlProc.OutputReceived += (o, e) => { stdOutput += e.Data + "\n"; };

            OptionSet options = new()
            {
                Quiet = true,
                NoWarnings = true,
                SkipDownload = true,
            };
            options.AddCustomOption<string>("--print", "width");
            options.AddCustomOption<string>("--print", "height");

            string[] urls = [url];
            int success =  await ytdlProc.RunAsync(urls, options);

            if(success > 0)
            {
                throw new Exception($"Failed to fetch video dimensions.");
            }

            string[] lines = stdOutput.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            if(lines.Length < 2)
            {
                return new(0, 0);
            }

            return new(int.Parse(lines[0]), int.Parse(lines[1]));
        }

        public async Task<VideoData> GetVideoDataAsync(string url)
        {
            if(!Uri.IsWellFormedUriString(url, UriKind.Absolute))
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

            if(!runResult.Success)
            {
                throw new Exception($"YtDlp failed with error: {runResult.ErrorOutput}");
            }

            return runResult.Data;
        }

        public async Task DownloadVideoAsync(string url, string videoName, VideoDimensions? dimensions, CancellationToken cancellationToken = default)
        {
            string filename = videoName + ".mp4";
            Video videoRecord = await videosService.CreateInitialVideoRecord(url, filename);

            try
            {
                if(!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    throw new Exception($"Invalid url {url}");
                }

                VideosInProgressStorage.AddVideoToTrack(videoRecord.Id);

                YoutubeDL ytdl = new()
                {
                    YoutubeDLPath = DependenciesHelper.YtDlpPath,
                    FFmpegPath = DependenciesHelper.FfmpegPath,
                    OutputFolder = AppConstants.DefaultDownloadDir
                };

                string requestedFormat;
                if(dimensions != null && dimensions.Height > 0 && dimensions.Width > 0)
                {
                    requestedFormat = $"bestvideo[height={dimensions.Height}][width={dimensions.Width}]+bestaudio/best";
                }
                else
                {
                    requestedFormat = "bestvideo+bestaudio/best";
                }

                Progress<DownloadProgress> progress = new(x =>
                {
                    VideosInProgressStorage.UpdateVideoPercent(videoRecord.Id, 100 * x.Progress);
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

                RunResult<string> result = await ytdl.RunVideoDownload(url, progress: progress, ct: cancellationToken, overrideOptions: options);

                if(!result.Success)
                {
                    var errorString = string.Join("\n", result.ErrorOutput);
                    throw new Exception(errorString);
                }

                string finalFilePath = Path.Join(AppConstants.DefaultDownloadDir, filename);
                videoRecord.Downloaded = true;
                videoRecord.ThumbnailName = videoName + ".jpg";
                videoRecord.Size = new FileInfo(finalFilePath).Length;
                videoRecord.UpdatedAt = DateTime.UtcNow;

                await videoDbContext.SaveChangesAsync();
                VideosInProgressStorage.MarkVideoDownloaded(videoRecord.Id);
            }
            catch(Exception ex)
            {
                string finalFilePath = Path.Join(AppConstants.DefaultDownloadDir, filename);
                string finalThumbnailPath = Path.Join(AppConstants.DefaultDownloadDir, videoName + ".jpg");

                if(File.Exists(finalFilePath))
                {
                    File.Delete(finalFilePath);
                }

                if(File.Exists(finalThumbnailPath))
                {
                    File.Delete(finalThumbnailPath);
                }

                if(videoRecord != null)
                {
                    videoDbContext.Remove(videoRecord);
                    await videoDbContext.SaveChangesAsync();
                }
                
                throw new Exception($"Failed to download video due to ex: {ex}");
            }
            
        }
    }
}
