#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
namespace YT_DLP_Web_App_Backend.DataObjects.Requests
{
    public class UpdateVideoNameRequest
    {
        public int VideoId { get; set; }
        public string NewName { get; set; }
    }
}
