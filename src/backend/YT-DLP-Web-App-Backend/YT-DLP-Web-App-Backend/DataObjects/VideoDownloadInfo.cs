using System.Text.Json.Serialization;
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
        [JsonPropertyName("videoId")]
        public int VideoId { get; set; }
        [JsonPropertyName("downloadPercent")]
        public float DownloadPercent { get; set; }
        [JsonPropertyName("downloaded")]
        public bool Downloaded { get; set; }
    }
}
