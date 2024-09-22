using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace YT_DLP_Web_App_Backend.DataObjects.Requests
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public class SaveVideoRequest
    {
        [Required]
        public string VideoUrl { get; set; }
        [Required]
        public string VideoName { get; set; }
        public VideoDimensions? VideoDimensions { get; set; }
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
}
