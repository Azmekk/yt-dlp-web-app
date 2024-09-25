using System.ComponentModel.DataAnnotations;

namespace YT_DLP_Web_App_Backend.Database.Entities
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public class Video
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string FileName { get; set; }
        public string? Mp3FileName { get; set; }
        public string ThumbnailName { get; set; }
        public long Size { get; set; }
        public string Url { get; set; }
        public bool Downloaded { get; set; }
        
        public Mp3Status Mp3Status { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    }

    public enum Mp3Status
    {
        None = 0,
        InProgress,
        Completed
    }
}
