using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using YT_DLP_Web_App_Backend.Constants;
using YT_DLP_Web_App_Backend.Database.Entities;

namespace YT_DLP_Web_App_Backend.Database
{
    public class VideoDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Video> Videos { get; set; }
    }
}
