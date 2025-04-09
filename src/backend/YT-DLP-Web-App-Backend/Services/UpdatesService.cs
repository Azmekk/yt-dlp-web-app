using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using YT_DLP_Web_App_Backend.Database;
using YT_DLP_Web_App_Backend.Database.Entities;
using YT_DLP_Web_App_Backend.Helpers;

namespace YT_DLP_Web_App_Backend.Services;

public class UpdatesService(VideoDbContext videoDbContext, HttpClient httpClient)
{
    public async Task CheckForYtDlpUpdateAndUpdateIfNeeded()
    {
        const string updateUrl = "https://api.github.com/repos/yt-dlp/yt-dlp/releases/latest";

        var response = await httpClient.GetAsync(updateUrl);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Failed to check for updates");
        }

        var content = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content);

        var publishDateString = json.RootElement.GetProperty("published_at").GetString() ??
                                throw new NullReferenceException("published_at is null");
        var publishDate = DateTime.Parse(publishDateString);

        var lastUpdate = await videoDbContext.AppUpdates.Where(x => x.App == UpdateApp.YtDlp)
            .OrderByDescending(x => x.Date).FirstOrDefaultAsync();
        
        if (lastUpdate is not null && lastUpdate.Date >= publishDate)
        {
            return;
        }

        await UpdateYtDlp();
    }
    
    public async Task UpdateYtDlp()
    {
       var sucess= await DependenciesHelper.UpdateYtDlpAsync();
       
        //UpdateYtDlpAsync will handle logging
        if (!sucess)
        {
            return;
        }

        var newUpdate = new AppUpdate
        {
            Date = DateTime.UtcNow,
            App = UpdateApp.YtDlp,
        };

        await videoDbContext.AppUpdates.AddAsync(newUpdate);
        await videoDbContext.SaveChangesAsync();
    }
}