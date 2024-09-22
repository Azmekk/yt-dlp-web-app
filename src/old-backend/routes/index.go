package routes

import (
	"net/http"
	"yt-dlp/web/routes/api"
	"yt-dlp/web/utils"
)

func RegisterRoutes() {
	api.RegisterApiRoutes()

	http.Handle("/", http.FileServer(http.Dir(utils.DefaultStaticDir)))
}
