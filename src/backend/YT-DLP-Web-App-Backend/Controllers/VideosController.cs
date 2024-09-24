using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Net;
using YT_DLP_Web_App_Backend.Constants;
using YT_DLP_Web_App_Backend.Database.Entities;
using YT_DLP_Web_App_Backend.DataObjects;
using YT_DLP_Web_App_Backend.DataObjects.Requests;
using YT_DLP_Web_App_Backend.DataObjects.Responses;
using YT_DLP_Web_App_Backend.Services;

namespace YT_DLP_Web_App_Backend.Controllers
{
    [Route("api/[controller]/[action]")]
    public class VideosController(VideosService videosService, YtDlpService ytDlpService) : Controller
    {
        [HttpPost]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> SaveVideo([FromBody] SaveVideoRequest request)
        {

            if(request.VideoName.Any(x => Path.GetInvalidFileNameChars().Contains(x)))
            {
                return BadRequest("Videoname contains invalid characters");
            }

            if(await videosService.VideoExists(request.VideoUrl))
            {
                return BadRequest("Video already exists");
            }

            if(videosService.VideoFileExists(request.VideoName + ".mp4"))
            {
                return BadRequest("A file with that name already exists");
            }

            //Create video record so it exists on returned request.
            Video video = await videosService.CreateInitialVideoRecord(request.VideoUrl, request.VideoName + ".mp4");
            BackgroundJob.Enqueue(() => ytDlpService.DownloadVideoAsync(request.VideoUrl, request.VideoName, video.Id, request.VideoDimensions, default));

            return Ok();
        }

        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<Video>))]
        public async Task<ActionResult<List<Video>>> ListVideos([Required] int take, [Required] int page, OrderVideosBy orderBy = OrderVideosBy.CreationDate, bool descending = true, string? search = "")
        {
            return Ok(await videosService.GetVideosAsync(take, page, orderBy, descending, search));
        }

        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(VideoCountResponse))]
        public async Task<ActionResult<VideoCountResponse>> GetVideoCount()
        {
            return Ok(new VideoCountResponse { Count = await videosService.GetVideoCount() });
        }

        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetVideo([Required] string videoName)
        {
            byte[]? fileBytes = await videosService.GetFileFromDownloadDir(videoName);

            if(fileBytes == null || fileBytes.Length == 0)
            {
                return NotFound();
            }

            return File(fileBytes, "video/mp4", videoName);
        }

        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetThumbnail([Required] string thumbnailName)
        {
            byte[]? fileBytes = await videosService.GetFileFromDownloadDir(thumbnailName);

            if(fileBytes == null || fileBytes.Length == 0)
            {
                return NotFound();
            }

            return File(fileBytes, "image/jpg", thumbnailName);
        }

        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(VideoNameResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<VideoNameResponse>> GetName(string videoUrl)
        {
            if(!Uri.IsWellFormedUriString(videoUrl, UriKind.Absolute))
            {
                return BadRequest($"Invalid url {videoUrl}");
            }

            YoutubeDLSharp.Metadata.VideoData videoData = await ytDlpService.GetVideoDataAsync(videoUrl);

            return Ok(new VideoNameResponse { Name = videoData.Title });
        }

        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(VideoDimensions))]
        public async Task<ActionResult<VideoDimensions>> GetMaxDimensions(string videoUrl)
        {
            VideoDimensions videoDimensions = await ytDlpService.GetMaxVideoResolutionAsync(videoUrl);

            return Ok(videoDimensions);
        }

        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> GetMp3([Required] int videoId)
        {
            try
            {
                (byte[]? fileBytes, string? mp3Name) = await videosService.ExtractMp3(videoId);
                if(fileBytes == null || fileBytes.Length == 0)
                {
                    return NotFound();
                }

                return File(fileBytes, "audio/mp3", mp3Name ?? "audio.mp3");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "An unexpected error occurred when converting to mp3.");
            }
        }


        [HttpDelete]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> DeleteVideo([Required] int videoId)
        {
            try
            {
                await videosService.DeleteVideo(videoId);
                return Ok();
            }
            catch(Exception)
            {
                return StatusCode(500, "An unexpected error occurred when deleting video.");
            }
        }

        [HttpPatch]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Video))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<Video>> UpdateVideoName([FromBody] UpdateVideoNameRequest updateNameRequest)
        {
            if(updateNameRequest.NewName.Any(x => Path.GetInvalidFileNameChars().Contains(x)))
            {
                return BadRequest("Videoname contains invalid characters");
            }

            Video? video = await videosService.UpdateVideoName(updateNameRequest.VideoId, updateNameRequest.NewName);

            if(video == null)
            {
                return NotFound();
            }

            return Ok(video);
        }

        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Video))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Video>> GetVideoInfo([Required] int videoId)
        {
            Video? video = await videosService.GetVideoById(videoId);

            if(video == null)
            {
                return NotFound();
            }

            return Ok(video);
        }

        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(VideoDownloadInfo))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public ActionResult<VideoDownloadInfo> GetVideoDownloadInfo([Required] int videoId)
        {
            var result = VideosInProgressStorage.GetVideoInfo(videoId);

            if(result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
