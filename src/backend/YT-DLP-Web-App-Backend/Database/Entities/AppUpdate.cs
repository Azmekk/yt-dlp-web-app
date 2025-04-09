using System.ComponentModel.DataAnnotations;

namespace YT_DLP_Web_App_Backend.Database.Entities;

public class AppUpdate
{
    [Key]
    public int Id { get; set; }
    
    public DateTime Date { get; set; }
    public UpdateApp App { get; set; }
}

public enum UpdateApp
{
    YtDlp,
    Ffmpeg,
}