namespace YT_DLP_Web_App_Backend.DataObjects.Requests
{
    public class UpdateVideoNameRequest
    {
        public int VideoId { get; set; }
        public string NewName { get; set; }
    }
}
