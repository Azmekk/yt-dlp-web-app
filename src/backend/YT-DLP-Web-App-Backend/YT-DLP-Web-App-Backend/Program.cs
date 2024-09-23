using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using YT_DLP_Web_App_Backend.Constants;
using YT_DLP_Web_App_Backend.Database;
using YT_DLP_Web_App_Backend.Helpers;
using YT_DLP_Web_App_Backend.Services;

namespace YT_DLP_Web_App_Backend
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            if(!await DependenciesHelper.VerifyOrInstallDependenciesAsync())
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

            if(app.Environment.IsDevelopment())
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
            app.Run();
        }

        private static void CreateRequiredDirsIfNotExist()
        {
            Directory.CreateDirectory(AppConstants.SqliteFolderPath);
            Directory.CreateDirectory(AppConstants.DefaultDownloadDir);
        }

        private static void CreateDbIfNotExist()
        {
            if(!File.Exists(AppConstants.SqliteFilePath))
            {
                File.Create(AppConstants.SqliteFilePath).Close();
            }
        }

        private static void AddServices(WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<VideoDbContext>(options =>
            {
                options.UseSqlite($"Data Source={AppConstants.SqliteFilePath}");
            });

            builder.Services.AddScoped<VideosService>();
            builder.Services.AddScoped<YtDlpService>();
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
                                      policy.AllowCredentials();
                                      policy.WithOrigins("http://localhost:5173", "https://localhost:5173", "localhost:5173")
                                      .AllowAnyHeader()
                                      .AllowAnyMethod()
                                      .WithExposedHeaders();
#else
                                      policy.AllowCredentials();
                                      policy.WithOrigins("http://localhost:41001", "https://localhost:41001")
                                      .AllowAnyHeader()
                                      .AllowAnyMethod();
#endif
                                  });
            });
        }
    }
}
