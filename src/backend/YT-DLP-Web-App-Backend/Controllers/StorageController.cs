using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YT_DLP_Web_App_Backend.Constants;
using YT_DLP_Web_App_Backend.DataObjects.Responses;
using YT_DLP_Web_App_Backend.Helpers;

namespace YT_DLP_Web_App_Backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StorageController : ControllerBase
    {
        [HttpGet]
        public ActionResult<UsedStorageResponse> GetUsedStorage()
        {
            DirectoryInfo downloadDirInfo = new(AppConstants.DefaultDownloadDir);
            DirectoryInfo dbDirInfo = new(AppConstants.SqliteFolderPath);

            long finalSize = downloadDirInfo.GetSize() + dbDirInfo.GetSize();

            return Ok(new UsedStorageResponse { UsedStorage = finalSize });
        }
    }
}
