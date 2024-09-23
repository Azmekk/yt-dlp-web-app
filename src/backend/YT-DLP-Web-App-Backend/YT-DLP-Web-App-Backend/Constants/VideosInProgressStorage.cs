using YT_DLP_Web_App_Backend.Database.Entities;
using YT_DLP_Web_App_Backend.DataObjects;

namespace YT_DLP_Web_App_Backend.Constants
{
    public delegate void OnVideoDownloadUpdated(int videoId, VideoDownloadInfo videoInfo);

    public static class VideosInProgressStorage
    {
        private static OnVideoDownloadUpdated? onVideoDownloadUpdated; 
        private readonly static Dictionary<int, VideoDownloadInfo> videosInProgress = [];

        public static void AddVideoToTrack(int videoId)
        {
            lock(videosInProgress)
            {
                videosInProgress.Add(videoId, new VideoDownloadInfo(videoId));
            }
        }

        public static void UpdateVideoPercent(int videoId, float newPercent)
        {
            lock(videosInProgress)
            {
                if(videosInProgress.TryGetValue(videoId, out VideoDownloadInfo? videoDownloadInfo))
                {
                    if(videoDownloadInfo != null && newPercent > videoDownloadInfo.DownloadPercent)
                    {
                        videoDownloadInfo.DownloadPercent = newPercent;
                        onVideoDownloadUpdated?.Invoke(videoId, videoDownloadInfo);
                    }
                }
            }
        }

        public static void MarkVideoDownloaded(int videoId)
        {
            lock(videosInProgress)
            {
                if(videosInProgress.TryGetValue(videoId, out VideoDownloadInfo? videoDownloadInfo))
                {
                    if(videoDownloadInfo != null)
                    {
                        videoDownloadInfo.DownloadPercent = 100;
                        videoDownloadInfo.Downloaded = true;
                        onVideoDownloadUpdated?.Invoke(videoId, videoDownloadInfo);
                    }

                    videosInProgress.Remove(videoId);
                }

            }
        }

        public static VideoDownloadInfo? GetVideoInfo(int videoId)
        {
            lock(videosInProgress)
            {
                videosInProgress.TryGetValue(videoId, out VideoDownloadInfo? videoDownloadInfo);
                return videoDownloadInfo;
            }
        }

        public static void AddVideoDownloadUpdatedHandler(OnVideoDownloadUpdated handler)
        {
            onVideoDownloadUpdated += handler;
        }

        public static void RemoveVideoDownloadUpdatedHandler(OnVideoDownloadUpdated handler)
        {
            onVideoDownloadUpdated -= handler;
        }
    }
}
