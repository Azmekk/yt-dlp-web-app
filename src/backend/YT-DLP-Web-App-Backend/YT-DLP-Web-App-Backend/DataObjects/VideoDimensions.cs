namespace YT_DLP_Web_App_Backend.DataObjects
{
    public class VideoDimensions
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public VideoDimensions(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}
