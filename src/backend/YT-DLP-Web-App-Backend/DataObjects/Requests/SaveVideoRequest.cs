#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
using System.ComponentModel.DataAnnotations;

namespace YT_DLP_Web_App_Backend.DataObjects.Requests
{
    public class SaveVideoRequest
    {
        [Required]
        public string VideoUrl { get; set; }
        [Required]
        public string VideoName { get; set; }
        public VideoDimensions? VideoDimensions { get; set; }
        public VideoDuration? VideoDuration { get; set; }
    }
}
