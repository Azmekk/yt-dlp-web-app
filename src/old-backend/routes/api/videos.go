package api

import (
	"yt-dlp/web/handlers"
	"yt-dlp/web/utils"
)

func RegisterVideosApiRoutes() {
	utils.HandleFuncWithMiddleware("/api/videos/save", handlers.SaveVideoHandler)
	utils.HandleFuncWithMiddleware("/api/videos/list", handlers.ListVideosHandler)
	utils.HandleFuncWithMiddleware("/api/videos/getcount", handlers.GetVideosCount)
	utils.HandleFuncWithMiddleware("/api/videos/download", handlers.DownloadVideo)
	utils.HandleFuncWithMiddleware("/api/videos/getInfo", handlers.GetVideoInfo)
	utils.HandleFuncWithMiddleware("/api/videos/delete", handlers.DeleteVideoHandler)
	utils.HandleFuncWithMiddleware("/api/videos/rename", handlers.UpdateName)
	utils.HandleFuncWithMiddleware("/api/videos/video", handlers.GetVideo)
	utils.HandleFuncWithMiddleware("/api/videos/thumbnail", handlers.GetThumbnail)
	utils.HandleFuncWithMiddleware("/api/videos/getYoutubeName", handlers.GetYoutubeName)
	utils.HandleFuncWithMiddleware("/api/videos/getVideoDimensions", handlers.GetVideoDimensions)
	utils.HandleFuncWithMiddleware("/api/videos/getStorageInfo", handlers.GetStorageInfo)
	utils.HandleFuncWithMiddleware("/api/videos/downloadMp3", handlers.DownloadMp3)
}
