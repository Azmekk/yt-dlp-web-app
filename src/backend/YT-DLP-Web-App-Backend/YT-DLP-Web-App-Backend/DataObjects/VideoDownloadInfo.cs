using YT_DLP_Web_App_Backend.Database.Entities;

namespace YT_DLP_Web_App_Backend.DataObjects
{
    public class VideoDownloadInfo
    {
        public VideoDownloadInfo(int videoId)
        {
            VideoId = videoId;
            DownloadPercent = 0;
            Downloaded = false;
        }

        public int VideoId { get; set; }
        public float DownloadPercent { get; set; }
        public bool Downloaded { get; set; }
    }
}
