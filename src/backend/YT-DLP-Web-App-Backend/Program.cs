using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using YT_DLP_Web_App_Backend.Constants;
using YT_DLP_Web_App_Backend.Database;
using YT_DLP_Web_App_Backend.Helpers;
using YT_DLP_Web_App_Backend.Services;
using YT_DLP_Web_App_Backend.Services.BackgroundServices;

namespace YT_DLP_Web_App_Backend
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            if (!await DependenciesHelper.VerifyOrInstallDependenciesAsync())
            {
                throw new Exception("App dependencies not found.");
            }

            CreateRequiredDirsIfNotExist();
            CreateDbIfNotExist();

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            AddServices(builder);
            SetupCors(builder);

            var app = builder.Build();

            await SetupApp(app);
        }

        private static async Task SetupApp(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            await MigrateDatabase(app);

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthorization();
            
            app.MapControllers();

            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
            };

            app.UseWebSockets(webSocketOptions);
            app.UseCors("Default");
            
            app.UseFileServer(new FileServerOptions()
            {
                FileProvider = new PhysicalFileProvider(AppConstants.DefaultStaticDir),
                EnableDirectoryBrowsing = false,
            });
            
            app.UseFileServer(new FileServerOptions()
            {
                RequestPath = "/Downloads",
                FileProvider = new PhysicalFileProvider(AppConstants.DefaultDownloadDir),
                EnableDirectoryBrowsing = false,
            });
            
            app.Run("http://*:41001");
        }

        private static void CreateRequiredDirsIfNotExist()
        {
            Directory.CreateDirectory(AppConstants.SqliteFolderPath);
            Directory.CreateDirectory(AppConstants.DefaultDownloadDir);
            Directory.CreateDirectory((AppConstants.DefaultStaticDir));
        }

        private static void CreateDbIfNotExist()
        {
            if (!File.Exists(AppConstants.SqliteFilePath))
            {
                File.Create(AppConstants.SqliteFilePath).Close();
            }
        }

        private static void AddServices(WebApplicationBuilder builder)
        {
            builder.Services.AddHangfire(config =>
                config.UseMemoryStorage());
            builder.Services.AddHangfireServer();

            builder.Services.AddDbContext<VideoDbContext>(options =>
            {
                options.UseSqlite($"Data Source={AppConstants.SqliteFilePath}");
            });

            builder.Services.AddScoped<VideosService>();
            builder.Services.AddScoped<YtDlpService>();

            builder.Services.AddHostedService<EnqueueUnfinishedVideosService>();
        }

        private static async Task MigrateDatabase(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            VideoDbContext dbContext = scope.ServiceProvider.GetRequiredService<VideoDbContext>();
            await dbContext.Database.MigrateAsync();
        }

        private static void SetupCors(WebApplicationBuilder builder)
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: "Default",
                    policy =>
                    {
#if DEBUG
                        policy.AllowCredentials()
                            .SetIsOriginAllowed(host => true)
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .WithExposedHeaders();
#else
                        policy.AllowCredentials()
                            .SetIsOriginAllowed(host => true)
                            .AllowAnyHeader()
                            .AllowAnyMethod();
#endif
                    });
            });
        }
    }
}