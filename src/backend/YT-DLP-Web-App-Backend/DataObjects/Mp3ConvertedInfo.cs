using System.Text.Json.Serialization;

namespace YT_DLP_Web_App_Backend.DataObjects;

public class Mp3ConvertedInfo(int videoId)
{
    [JsonPropertyName("videoId")] public int VideoId { get; set; } = videoId;

    [JsonPropertyName("converted")] public bool Converted { get; set; } = false;
}