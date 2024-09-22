using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using YT_DLP_Web_App_Backend.Database.Entities;
using YT_DLP_Web_App_Backend.DataObjects.Requests;
using YT_DLP_Web_App_Backend.DataObjects.Responses;
using YT_DLP_Web_App_Backend.Services;

namespace YT_DLP_Web_App_Backend.Controllers
{
    [Route("api/[controller]/[action]")]
    public class VideosController(VideosService videosService, YtDlpService ytDlpService) : Controller
    {
        [HttpPost]
        public async Task<ActionResult> SaveVideo([FromBody] SaveVideoRequest request)
        {
            var proceedWithDownload = true;
            try
            {
                if(request.VideoName.Any(x => Path.GetInvalidFileNameChars().Contains(x)))
                {
                    proceedWithDownload = false;
                    return BadRequest("Videoname contains invalid characters");
                }

                if(await videosService.VideoExists(request.VideoUrl))
                {
                    proceedWithDownload = false;
                    return BadRequest("Video already exists");
                }

                if(videosService.VideoFileExists(request.VideoName + ".mp4"))
                {
                    proceedWithDownload = false;
                    return BadRequest("A file with that name already exists");
                }

                return Ok();
            }
            finally
            {
                Response.OnCompleted(async () =>
                {
                    if(proceedWithDownload)
                    {
                        await ytDlpService.DownloadVideoAsync(request.VideoUrl, request.VideoName, request.VideoDimensions);
                    }
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<Video>>> ListVideos([Required] int take, [Required] int page, OrderVideosBy orderBy = OrderVideosBy.CreationDate, bool descending = true, string search = "")
        {
            return Ok(await videosService.GetVideosAsync(take, page, orderBy, descending, search));
        }

        [HttpGet]
        public async Task<ActionResult<VideoCountResponse>> GetVideoCount()
        {
            return Ok(new VideoCountResponse { Count = await videosService.GetVideoCount() });
        }

        [HttpGet]
        public ActionResult GetVideo([Required] string videoName)
        {
            FileStream? fs = videosService.GetFileFromDownloadDir(videoName);

            if(fs == null)
            {
                return NotFound();
            }

            return File(fs, "video/mp4", videoName);
        }

        [HttpGet]
        public ActionResult GetThumbnail([Required] string thumbnailName)
        {
            FileStream? fs = videosService.GetFileFromDownloadDir(thumbnailName);

            if(fs == null)
            {
                return NotFound();
            }

            return File(fs, "image/jpg", thumbnailName);
        }

        [HttpGet]
        public async Task<ActionResult<VideoNameResponse>> GetName(string videoUrl)
        {
            YoutubeDLSharp.Metadata.VideoData videoData = await ytDlpService.GetVideoDataAsync(videoUrl);

            return Ok(new VideoNameResponse { Name = videoData.Title });
        }

        [HttpGet]
        public async Task<ActionResult<VideoNameResponse>> GetMaxDimensions(string videoUrl)
        {
            DataObjects.VideoDimensions videoDimensions = await ytDlpService.GetMaxVideoResolutionAsync(videoUrl);

            return Ok(videoDimensions);
        }

        [HttpGet]
        public async Task<ActionResult> GetMp3([Required] string videoName)
        {
            try
            {
                FileStream? fs = await videosService.ExtractMp3(videoName);
                if(fs == null)
                {
                    return NotFound();
                }

                string mp3Name = videoName.TrimEnd(Path.GetExtension(videoName)) + ".mp3";
                return File(fs, "audio/mp3", mp3Name);
            }
            catch(Exception)
            {
                return StatusCode(500, "An unexpected error occurred when converting to mp3.");
            }
        }


        [HttpDelete]
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
        public async Task<ActionResult<Video>> GetVideoInfo([Required] int videoId)
        {
            Video? video = await videosService.GetVideoById(videoId);

            if(video == null)
            {
                return NotFound();
            }

            return Ok(video);
        }
    }
}
