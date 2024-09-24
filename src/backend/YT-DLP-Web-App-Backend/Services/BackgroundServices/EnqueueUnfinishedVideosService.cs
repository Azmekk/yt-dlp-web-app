using Hangfire;
using Microsoft.EntityFrameworkCore;
using YT_DLP_Web_App_Backend.Database;
using YT_DLP_Web_App_Backend.Database.Entities;

namespace YT_DLP_Web_App_Backend.Services.BackgroundServices;

public class EnqueueUnfinishedVideosService(IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = serviceScopeFactory.CreateScope();
        VideoDbContext dbContext = scope.ServiceProvider.GetRequiredService<VideoDbContext>();
        YtDlpService ytDlpService = scope.ServiceProvider.GetRequiredService<YtDlpService>();

        List<Video> videos = await dbContext.Videos.Where(x => x.Downloaded == false).ToListAsync();

        foreach (var video in videos)
        {
            BackgroundJob.Enqueue(() => ytDlpService.DownloadSavedVideo(video.Id));
        }
        
    }
}