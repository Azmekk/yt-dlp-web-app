using FFmpeg.NET;
using FFmpeg.NET.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using YT_DLP_Web_App_Backend.Constants;
using YT_DLP_Web_App_Backend.Database;
using YT_DLP_Web_App_Backend.Database.Entities;
using YT_DLP_Web_App_Backend.Helpers;

namespace YT_DLP_Web_App_Backend.Services
{
    public class VideosService(VideoDbContext videoDbContext)
    {
        public async Task<Video> CreateInitialVideoRecord(string url, string name)
        {
            Video video = new()
            {
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                DeletedAt = null,
                FileName = name,
                ThumbnailName = "",
                Size = 0,
                Url = url,
                Downloaded = false,
            };

            await videoDbContext.AddAsync(video);
            await videoDbContext.SaveChangesAsync();

            return video;
        }

        public async Task<List<Video>> GetVideosAsync(int take, int page, OrderVideosBy orderBy, bool descending, string search = "")
            {
            var queryable = videoDbContext.Videos
                .Where(x => x.DeletedAt == null);

            if(!string.IsNullOrEmpty(search))
            {
                queryable = queryable.Where(x => x.FileName.Contains(search));
            }

            switch(orderBy)
            {
                case OrderVideosBy.CreationDate:
                    if(descending)
                    {
                        queryable = queryable.OrderByDescending(x => x.CreatedAt);
                        break;
                    }
                    else
                    {
                        queryable = queryable.OrderBy(x => x.CreatedAt);
                        break;
                    }
                case OrderVideosBy.FileName:
                    if(descending)
                    {
                        queryable = queryable.OrderByDescending(x => x.CreatedAt);
                        break;
                    }
                    else
                    {
                        queryable = queryable.OrderBy(x => x.CreatedAt);
                        break;
                    }
                case OrderVideosBy.Size:
                    if(descending)
                    {
                        queryable = queryable.OrderByDescending(x => x.CreatedAt);
                        break;
                    }
                    else
                    {
                        queryable = queryable.OrderBy(x => x.CreatedAt);
                        break;
                    }
            }

            queryable = queryable
                .Skip((page - 1) * take)
                .Take(take);

            List<Video> result = await queryable.ToListAsync();
            return result;
        }

        public async Task<int> GetVideoCount()
        {
            return await videoDbContext.Videos.CountAsync(x => x.DeletedAt == null);
        }

        public async Task<Video?> GetVideoById(int videoId)
        {
            return await videoDbContext.Videos.FirstOrDefaultAsync(x => x.DeletedAt == null && x.Id == videoId);
        }

        public async Task DeleteVideo(int videoId)
        {
            Video? video = await videoDbContext.Videos.FirstOrDefaultAsync(x => x.Id == videoId);

            if(video == null)
            {
                return;
            }

            var videoPath = Path.Join(AppConstants.DefaultDownloadDir, video.FileName);
            if(File.Exists(videoPath))
            {
                File.Delete(videoPath);
            }

            var thumbnailPath = Path.Join(AppConstants.DefaultDownloadDir, video.ThumbnailName);
            if(File.Exists(thumbnailPath))
            {
                File.Delete(thumbnailPath);
            }

            videoDbContext.Remove(video);
            await videoDbContext.SaveChangesAsync();
        }

        public async Task<byte[]?> GetFileFromDownloadDir(string name)
        {
            var filePath = Path.Join(AppConstants.DefaultDownloadDir, name);

            if(!File.Exists(filePath))
            {
                return null;
            }

            return await File.ReadAllBytesAsync(filePath);
        }

        public async Task<byte[]?> ExtractMp3(string videoName)
        {
            string videoNameNoExtension = videoName.TrimEnd(Path.GetExtension(videoName));
            string mp3Name = videoNameNoExtension + ".mp3";

            string mp3Path = Path.Join(AppConstants.DefaultDownloadDir, mp3Name);
            if(File.Exists(mp3Path))
            {
                return await File.ReadAllBytesAsync(mp3Path);
            }

            string videoPath = Path.Join(AppConstants.DefaultDownloadDir, videoName);
            if(!File.Exists(videoPath))
            {
                return null;
            }

            Engine ffmpeg = new(DependenciesHelper.FfmpegPath);
            await ffmpeg.ExecuteAsync($"-i \"{videoPath}\" -q:a 0 -map 0:a \"{mp3Path}\"", default);

            return await File.ReadAllBytesAsync(mp3Path);
        }

        public async Task<Video?> UpdateVideoName(int videoId, string newName)
        {
            Video? video = await videoDbContext.Videos.FirstOrDefaultAsync(x => x.Id == videoId);

            if(video == null)
            {
                return null;
            }

            var videoPath = Path.Join(AppConstants.DefaultDownloadDir, video.FileName);
            var newVideoPath = Path.Join(AppConstants.DefaultDownloadDir, newName);
            if(File.Exists(videoPath))
            {
                File.Move(videoPath, newVideoPath);
            }

            string videoNameNoExtension = newName.TrimEnd(Path.GetExtension(newName));
            string newThumbnailName = videoNameNoExtension + Path.GetExtension(video.ThumbnailName);

            var thumbnailPath = Path.Join(AppConstants.DefaultDownloadDir, video.ThumbnailName);
            var newThumbnailPath = Path.Join(AppConstants.DefaultDownloadDir, newThumbnailName);
            if(File.Exists(thumbnailPath))
            {
                File.Move(thumbnailPath, newThumbnailPath);
            }

            video.FileName = newName;
            video.ThumbnailName = newThumbnailName;

            await videoDbContext.SaveChangesAsync();

            return video;
        }

        public async Task<bool> VideoExists(string url)
        {
            Video? existingVideo = await videoDbContext.Videos.FirstOrDefaultAsync(x => x.Url == url);

            if(existingVideo != null)
            {
                return true;
            }

            return false;
        }

        public bool VideoFileExists(string videoName)
        {
            return File.Exists(Path.Join(AppConstants.DefaultDownloadDir, videoName));
        }
    }

    public enum OrderVideosBy
    {
        CreationDate,
        FileName,
        Size
    }
}
