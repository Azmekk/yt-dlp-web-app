using YT_DLP_Web_App_Backend.DataObjects;

namespace YT_DLP_Web_App_Backend.Constants
{
    public delegate void OnVideoDownloadUpdated(VideoDownloadInfo videoInfo);
    public delegate void OnMp3Converted(Mp3ConvertedInfo mp3ConvertedInfo);

    public static class MediaInProgressStorage
    {
        private static OnVideoDownloadUpdated? _onVideoDownloadUpdated;
        private static OnMp3Converted? _onMp3Converted;
        private static readonly Dictionary<int, VideoDownloadInfo> VideosInProgress = [];

        public static void AddVideoToTrack(int videoId)
        {
            lock (VideosInProgress)
            {
                VideosInProgress.Add(videoId, new VideoDownloadInfo(videoId));
            }
        }

        public static void UpdateVideoPercent(int videoId, float newPercent)
        {
            lock (VideosInProgress)
            {
                if (VideosInProgress.TryGetValue(videoId, out VideoDownloadInfo? videoDownloadInfo) &&
                    videoDownloadInfo != null && newPercent > videoDownloadInfo.DownloadPercent)
                {
                    videoDownloadInfo.DownloadPercent = newPercent;
                    _onVideoDownloadUpdated?.Invoke(videoDownloadInfo);
                }
            }
        }

        public static void MarkVideoDownloaded(int videoId)
        {
            lock (VideosInProgress)
            {
                if (VideosInProgress.TryGetValue(videoId, out VideoDownloadInfo? videoDownloadInfo))
                {
                    if (videoDownloadInfo != null)
                    {
                        videoDownloadInfo.DownloadPercent = 100;
                        videoDownloadInfo.Downloaded = true;
                        _onVideoDownloadUpdated?.Invoke(videoDownloadInfo);
                    }

                    VideosInProgress.Remove(videoId);
                }
            }
        }

        public static void MarkVideoFailed(int videoId)
        {
            lock(VideosInProgress)
            {
                if(VideosInProgress.TryGetValue(videoId, out VideoDownloadInfo? videoDownloadInfo))
                {
                    if(videoDownloadInfo != null)
                    {
                        videoDownloadInfo.DownloadPercent = -1;
                        videoDownloadInfo.Downloaded = false;
                        _onVideoDownloadUpdated?.Invoke(videoDownloadInfo);
                    }

                    VideosInProgress.Remove(videoId);
                }
            }
        }


        public static void MarkMp3Converted(int videoId)
        {
            lock (VideosInProgress)
            {
                _onMp3Converted?.Invoke(new Mp3ConvertedInfo(videoId));
            }
        }

        public static VideoDownloadInfo? GetVideoInfo(int videoId)
        {
            lock (VideosInProgress)
            {
                VideosInProgress.TryGetValue(videoId, out VideoDownloadInfo? videoDownloadInfo);
                return videoDownloadInfo;
            }
        }

        public static void AddVideoDownloadUpdatedHandler(OnVideoDownloadUpdated handler)
        {
            _onVideoDownloadUpdated += handler;
        }

        public static void RemoveVideoDownloadUpdatedHandler(OnVideoDownloadUpdated handler)
        {
            _onVideoDownloadUpdated -= handler;
        }

        public static void AddMp3ConversionUpdateHandler(OnMp3Converted handler)
        {
            _onMp3Converted += handler;
        }
        
        public static void RemoveM3ConversionUpdateHandler(OnMp3Converted handler)
        {
            _onMp3Converted -= handler;
        }
    }
}