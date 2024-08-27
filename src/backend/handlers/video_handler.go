package handlers

import (
	"bufio"
	"encoding/json"
	"fmt"
	"math"
	"net/http"
	"net/url"
	"os"
	"os/exec"
	"path/filepath"
	"regexp"
	"strconv"
	"strings"
	"time"
	"yt-dlp/web/database"
	"yt-dlp/web/utils"

	"gorm.io/gorm"
)

type DriveUsageResponse struct {
	UsedStorage int64 `json:"usedStorage"`
}

type VideoCountResponse struct {
	Count int `json:"count"`
}

type VideoResponse struct {
	Id              uint      `json:"id"`
	CreationTime    time.Time `json:"creation_time"`
	Name            string    `json:"name"`
	ThumbnailName   string    `json:"thumbnailName"`
	Size            int64     `json:"size"`
	Url             string    `json:"url"`
	Downloaded      bool      `json:"downloaded"`
	DownloadPercent int       `json:"downloadPercent"`
}

type VideoListResponse struct {
	Videos      []VideoResponse `json:"videos"`
	TotalAmount int             `json:"totalAmount"`
}

type TitleResponse struct {
	Title string `json:"title"`
}

type NoembedInfo struct {
	ThumbnailHeight int    `json:"thumbnail_height"`
	Version         string `json:"version"`
	URL             string `json:"url"`
	ProviderURL     string `json:"provider_url"`
	Height          int    `json:"height"`
	Type            string `json:"type"`
	HTML            string `json:"html"`
	AuthorName      string `json:"author_name"`
	AuthorURL       string `json:"author_url"`
	ThumbnailURL    string `json:"thumbnail_url"`
	Title           string `json:"title"`
	Width           int    `json:"width"`
	ThumbnailWidth  int    `json:"thumbnail_width"`
	ProviderName    string `json:"provider_name"`
}

type VideoDimensions struct {
	Width  int `json:"width"`
	Height int `json:"height"`
}

var resolutions = map[string]VideoDimensions{
	"144p":  {Width: 256, Height: 144},
	"240p":  {Width: 426, Height: 240},
	"360p":  {Width: 640, Height: 360},
	"480p":  {Width: 854, Height: 480},
	"720p":  {Width: 1280, Height: 720},
	"1080p": {Width: 1920, Height: 1080},
	"1440p": {Width: 2560, Height: 1440},
	"2160p": {Width: 3840, Height: 2160},
}

func SaveVideoHandler(w http.ResponseWriter, r *http.Request) {
	query := r.URL.Query()
	videoUrl := query.Get("video")
	requestedFileName := query.Get("name")
	requestedFormat := query.Get("format")
	requestedResolution := query.Get("resolution")

	u, err := url.Parse(videoUrl)

	if err != nil || u.Scheme == "" || u.Host == "" {
		utils.WriteJsonError(w, "Video param must be provided and be a valid url", http.StatusBadRequest)
		return
	}

	var videoDimensions VideoDimensions = VideoDimensions{
		Height: 0,
		Width:  0,
	}
	if requestedResolution != "" {
		foundDimensions, exists := resolutions[requestedResolution]

		if !exists {
			utils.WriteJsonError(w, "Provided resolution not recognized. Api only accepts from 144p to 4k standard res.", http.StatusBadRequest)
			return
		}

		videoDimensions = foundDimensions
	}

	existingVideo, err := database.CheckForExistingVideo(videoUrl)
	if err != nil && err != gorm.ErrRecordNotFound {
		fmt.Println("Error when checking for existing video: ", err)
		utils.WriteJsonError(w, "Issue when checking for existing video.", http.StatusBadRequest)
		return
	}

	var videoRecord *database.Video
	if existingVideo != nil && !existingVideo.DeletedAt.Valid {
		utils.WriteJsonError(w, fmt.Sprintf("Video already exists under the name: %s", existingVideo.FileName), http.StatusConflict)
		return
	} else if existingVideo != nil && existingVideo.DeletedAt.Valid {
		fmt.Println("Restoring existing video: ", existingVideo.FileName)

		videoName := fmt.Sprintf("%s.%s", requestedFileName, requestedFormat)

		err = database.RestoreDeletedVideo(existingVideo)
		if err != nil {
			fmt.Println("Error when restoring existing video: ", err)
			utils.WriteJsonError(w, "Issue when restoring existing video.", http.StatusBadRequest)
			return
		}

		videoRecord = existingVideo
		videoRecord.FileName = videoName

	} else {
		videoName := fmt.Sprintf("%s.%s", requestedFileName, requestedFormat)
		videoRecord, err = database.CreateVideo(videoUrl, videoName)

		if err != nil {
			utils.WriteJsonError(w, "Failed to create an initial video", http.StatusBadRequest)
			return
		}
	}

	go createVideo(videoUrl, videoRecord, videoDimensions)

	response := convertVideoToVideoResponse(videoRecord)

	utils.WriteJsonResponse(response, w)

}

func ListVideosHandler(w http.ResponseWriter, r *http.Request) {
	query := r.URL.Query()

	take, err := strconv.Atoi(query.Get("take"))
	if err != nil {
		utils.WriteJsonError(w, "take parameter must be a valid integer", http.StatusBadRequest)
		return
	}

	page, err := strconv.Atoi(query.Get("page"))
	if err != nil {
		utils.WriteJsonError(w, "page parameter must be a valid integer", http.StatusBadRequest)
		return
	}

	orderBy, err := strconv.Atoi(query.Get("orderBy"))
	if err != nil {
		utils.WriteJsonError(w, "orderBy parameter must be a valid integer from 0 - 3 ", http.StatusBadRequest)
		return
	}

	descending, err := strconv.ParseBool(query.Get("descending"))
	if err != nil {
		utils.WriteJsonError(w, "descending parameter must be a valid boolean", http.StatusBadRequest)
		return
	}

	search := query.Get("search")
	offset := (page - 1) * take

	videoCount, err := database.GetVideosCountWithFilters(orderBy, descending, search)
	if err != nil {
		utils.WriteJsonError(w, "Something went wrong when counting videos", http.StatusInternalServerError)
		fmt.Println("Error when trying to count videos: ", err)
		return
	}

	if videoCount == 0 {
		utils.WriteJsonResponse([]VideoResponse{}, w)
		return
	}

	if int64(offset) >= videoCount {
		utils.WriteJsonError(w, "Invalid page and take arguments. Offset bigger than amount of videos.", http.StatusBadRequest)
		return
	}

	videos, err := database.GetExistingVideosWithFilters(take, offset, orderBy, descending, search)
	if err != nil {
		utils.WriteJsonError(w, "Something went wrong when trying to get videos", http.StatusInternalServerError)
		fmt.Println("Error when querying videos: ", err)
		return
	}

	response := new(VideoListResponse)
	response.TotalAmount = int(videoCount)

	var responseVideos []VideoResponse
	for i := 0; i < len(*videos); i++ {
		videoJson := convertVideoToVideoResponse(&(*videos)[i])

		responseVideos = append(responseVideos, videoJson)
	}
	response.Videos = responseVideos

	utils.WriteJsonResponse(response, w)

}

func GetVideosCount(w http.ResponseWriter, r *http.Request) {
	videoCount, err := database.GetVideosCount()
	if err != nil {
		utils.WriteJsonError(w, "Something went wrong when counting videos", http.StatusInternalServerError)
		fmt.Println("Error when trying to count videos: ", err)
		return
	}

	response := VideoCountResponse{
		Count: int(videoCount),
	}

	utils.WriteJsonResponse(response, w)
}

func DownloadVideo(w http.ResponseWriter, r *http.Request) {
	query := r.URL.Query()
	videoId, err := strconv.Atoi(query.Get("id"))

	if err != nil || videoId < 1 {
		utils.WriteJsonError(w, "Please provide a valid video ID.", http.StatusBadRequest)
		return
	}

	video, err := database.GetVideoById(uint(videoId))

	if err == gorm.ErrRecordNotFound {
		utils.WriteJsonError(w, "Video not found.", http.StatusNotFound)
		return
	}

	if err != nil {
		utils.WriteJsonError(w, "Something went wrong when getting video", http.StatusInternalServerError)
		fmt.Println(fmt.Sprintf("Error when trying to fetch video with ID %d from database: ", videoId), err)
		return
	}

	utils.FileDownloadResponse(w, r, filepath.Join(utils.DefaultDownloadDir, video.FileName))
}

func DownloadMp3(w http.ResponseWriter, r *http.Request) {
	query := r.URL.Query()
	videoId, err := strconv.Atoi(query.Get("id"))

	if err != nil || videoId < 1 {
		utils.WriteJsonError(w, "Please provide a valid video ID.", http.StatusBadRequest)
		return
	}

	video, err := database.GetVideoById(uint(videoId))

	if err == gorm.ErrRecordNotFound {
		utils.WriteJsonError(w, "Video not found.", http.StatusNotFound)
		return
	}

	if err != nil {
		utils.WriteJsonError(w, "Something went wrong when getting video", http.StatusInternalServerError)
		fmt.Println(fmt.Sprintf("Error when trying to fetch video with ID %d from database: ", videoId), err)
		return
	}

	videoNameNoExt := utils.GetFilenameWithoutExt(video.FileName)
	mp3FileName := fmt.Sprintf("%s.mp3", videoNameNoExt)

	cmd := exec.Command(utils.DefaultFfmpegCommand, "-i", filepath.Join(utils.DefaultDownloadDir, video.FileName), "-f", "mp3", filepath.Join(utils.DefaultDownloadDir, mp3FileName))
	if err := cmd.Run(); err != nil {
		utils.WriteJsonError(w, "Something went wrong when encoding mp3", http.StatusInternalServerError)
		fmt.Println("Something went wrong when encoding mp3: ", err)
		return
	}

	defer os.Remove(filepath.Join(utils.DefaultDownloadDir, mp3FileName))

	utils.FileDownloadResponse(w, r, filepath.Join(utils.DefaultDownloadDir, mp3FileName))
}

func GetVideo(w http.ResponseWriter, r *http.Request) {
	query := r.URL.Query()
	videoId, err := strconv.Atoi(query.Get("id"))

	if err != nil || videoId < 1 {
		utils.WriteJsonError(w, "Please provide a valid video ID.", http.StatusBadRequest)
		return
	}

	video, err := database.GetVideoById(uint(videoId))

	if err != nil {
		utils.WriteJsonError(w, "Something went wrong when getting video", http.StatusInternalServerError)
		fmt.Println(fmt.Sprintf("Error when trying to fetch video with ID %d from database: ", videoId), err)
		return
	}

	utils.FileResponse(w, r, filepath.Join(utils.DefaultDownloadDir, video.FileName))
}

func GetThumbnail(w http.ResponseWriter, r *http.Request) {
	query := r.URL.Query()
	videoId, err := strconv.Atoi(query.Get("id"))

	if err != nil || videoId < 1 {
		utils.WriteJsonError(w, "Please provide a valid video ID.", http.StatusBadRequest)
		return
	}

	video, err := database.GetVideoById(uint(videoId))

	if err != nil {
		utils.WriteJsonError(w, "Something went wrong when getting video thumbnail", http.StatusInternalServerError)
		fmt.Println(fmt.Sprintf("Error when trying to fetch video thumbnail with ID %d from database: ", videoId), err)
		return
	}

	utils.FileResponse(w, r, filepath.Join(utils.DefaultDownloadDir, video.ThumnailName))
}

func GetVideoInfo(w http.ResponseWriter, r *http.Request) {
	query := r.URL.Query()
	videoId, err := strconv.Atoi(query.Get("id"))

	if err != nil || videoId < 1 {
		utils.WriteJsonError(w, "Please provide a valid video ID.", http.StatusBadRequest)
		return
	}

	video, err := database.GetVideoById(uint(videoId))

	if err == gorm.ErrRecordNotFound {
		utils.WriteJsonError(w, "Record not found", http.StatusNotFound)
		return
	}

	if err != nil {
		utils.WriteJsonError(w, "Something went wrong when getting video", http.StatusInternalServerError)
		fmt.Println(fmt.Sprintf("Error when trying to fetch video with ID %d from database: ", videoId), err)
		return
	}

	response := convertVideoToVideoResponse(video)

	utils.WriteJsonResponse(response, w)
}

func DeleteVideoHandler(w http.ResponseWriter, r *http.Request) {
	query := r.URL.Query()
	videoId, err := strconv.Atoi(query.Get("id"))

	if err != nil || videoId < 1 {
		utils.WriteJsonError(w, "Please provide a valid video ID.", http.StatusBadRequest)
		return
	}

	video, err := database.GetVideoById(uint(videoId))

	if err == gorm.ErrRecordNotFound {
		utils.WriteJsonError(w, fmt.Sprintf("Video with id %d does not exist", videoId), http.StatusInternalServerError)
		return
	} else if err != nil {
		utils.WriteJsonError(w, "Something went wrong when getting video", http.StatusInternalServerError)
		fmt.Println(fmt.Sprintf("Error when trying to fetch video with ID %d from database: ", videoId), err)
		return
	}

	err = os.Remove(filepath.Join(utils.DefaultDownloadDir, video.FileName))

	if err != nil {
		utils.WriteJsonError(w, "Something went wrong when deleting video", http.StatusInternalServerError)
		fmt.Println(fmt.Sprintf("Error when trying to delete video with ID %d: ", videoId), err)
		return
	}

	database.DbConn.Delete(&video)

	err = os.Remove(filepath.Join(utils.DefaultDownloadDir, video.ThumnailName))

	if err != nil {
		fmt.Println(fmt.Sprintf("Error when trying to delete thumbnail for video with ID %d: ", videoId), err)
		return
	}

	utils.WriteEmptySuccess(w)
}

func UpdateName(w http.ResponseWriter, r *http.Request) {
	query := r.URL.Query()
	videoId, err := strconv.Atoi(query.Get("id"))

	if err != nil || videoId < 1 {
		utils.WriteJsonError(w, "Please provide a valid video ID.", http.StatusBadRequest)
		return
	}

	newName := query.Get("newName")
	if newName == "" {
		utils.WriteJsonError(w, "Please provide video name.", http.StatusBadRequest)
		return
	}

	video, err := database.GetVideoById(uint(videoId))

	if err == gorm.ErrRecordNotFound {
		utils.WriteJsonError(w, fmt.Sprintf("Video with id %d does not exist", videoId), http.StatusInternalServerError)
		return
	} else if err != nil {
		utils.WriteJsonError(w, "Something went wrong when getting video", http.StatusInternalServerError)
		fmt.Println(fmt.Sprintf("Error when trying to fetch video with ID %d from database: ", videoId), err)
		return
	}

	newFileName := filepath.Join(filepath.Join(utils.DefaultDownloadDir, newName))
	err = os.Rename(filepath.Join(utils.DefaultDownloadDir, video.FileName), newFileName)

	if err != nil {
		utils.WriteJsonError(w, "Something went wrong when renaming video", http.StatusInternalServerError)
		fmt.Println(fmt.Sprintf("Error when trying to rename video with ID %d: ", videoId), err)
		return
	}

	video.FileName = newName
	database.DbConn.Save(&video)

	utils.WriteEmptySuccess(w)
}

func GetYoutubeName(w http.ResponseWriter, r *http.Request) {
	query := r.URL.Query()
	videoUrl := query.Get("video")

	u, err := url.Parse(videoUrl)

	if err != nil || u.Scheme == "" || u.Host == "" {
		utils.WriteJsonError(w, "Video param must be provided and be a valid url", http.StatusBadRequest)
		return
	}

	fmt.Println("Fetching name for video: ", videoUrl)

	videoInfo := new(NoembedInfo)
	err = utils.FetchJson(fmt.Sprintf(`https://noembed.com/embed?url=%s`, videoUrl), videoInfo)

	if err != nil {
		utils.WriteJsonError(w, "Something went wrong when fetching info about the video", http.StatusInternalServerError)
		fmt.Println("Error when fetching info about the video: ", err)
		return
	}

	normalizedName := strings.Replace(videoInfo.Title, " ", "_", -1)

	fileTitleRegxp := regexp.MustCompile(utils.AllowedFileCharsRegex)
	normalizedName = fileTitleRegxp.ReplaceAllString(normalizedName, "")

	response := TitleResponse{
		Title: normalizedName,
	}

	utils.WriteJsonResponse(response, w)
}

func GetVideoDimensions(w http.ResponseWriter, r *http.Request) {
	query := r.URL.Query()
	videoUrl := query.Get("video")

	u, err := url.Parse(videoUrl)

	if err != nil || u.Scheme == "" || u.Host == "" {
		utils.WriteJsonError(w, "Video param must be provided and be a valid url", http.StatusBadRequest)
		return
	}

	width, height := utils.GetMaxVideoResolution(videoUrl)

	if width == 0 || height == 0 {
		utils.WriteJsonError(w, "Something went wrong when parsing the video.", http.StatusInternalServerError)
		return
	}

	response := VideoDimensions{
		Width:  width,
		Height: height,
	}

	utils.WriteJsonResponse(response, w)
}

func GetStorageInfo(w http.ResponseWriter, r *http.Request) {
	usedStorage, err := utils.GetUsedFoldersSize()
	if err != nil {
		utils.WriteJsonError(w, "Something went wrong when getting folder size.", http.StatusInternalServerError)
		return
	}

	response := DriveUsageResponse{
		UsedStorage: usedStorage,
	}

	utils.WriteJsonResponse(response, w)
}

func createVideo(videoUrl string, videoRecord *database.Video, dimensions VideoDimensions) {
	if videoRecord == nil {
		fmt.Println("Video record was nil")
		return
	}

	videoInfo := new(NoembedInfo)
	utils.FetchJson(fmt.Sprintf(`https://noembed.com/embed?url=%s`, videoUrl), videoInfo)

	fmt.Println("Attempting to download video: ", videoUrl)

	err := runYtDlpCommand(videoRecord, dimensions.Height)
	if err != nil {
		fmt.Println("Error when trying to download video: ", err)
		database.DeleteVideoById(videoRecord.ID)
		return
	}

	fileInfo, err := os.Stat(filepath.Join(utils.DefaultDownloadDir, videoRecord.FileName))
	if err != nil {
		fmt.Println(fmt.Sprintf("Error when trying to get video info for: %s", videoRecord.FileName), err)
		database.DeleteVideoById(videoRecord.ID)
		return
	}

	thumbnailName := utils.DownloadAndCreateThumbnail(videoInfo.ThumbnailURL, videoRecord.FileName)

	if thumbnailName == "" {
		thumbnailName = utils.CreateThumbnailFromFrame(filepath.Join(utils.DefaultDownloadDir, videoRecord.FileName))
	}

	err = database.MarkVideoDownloaded(videoRecord, videoRecord.FileName, thumbnailName, fileInfo.Size())
	if err != nil {
		fmt.Println("Error when trying to update video info: ", err)
		database.DeleteVideoById(videoRecord.ID)
	}
}

func convertVideoToVideoResponse(video *database.Video) VideoResponse {
	return VideoResponse{
		Id:              video.ID,
		CreationTime:    video.CreatedAt,
		Name:            video.FileName,
		ThumbnailName:   video.ThumnailName,
		Size:            video.Size,
		Url:             video.Url,
		Downloaded:      video.Downloaded,
		DownloadPercent: video.DownloadPercent,
	}
}

func runYtDlpCommand(video *database.Video, height int) error {
	args := []string{"--no-mtime"}
	if height != 0 {
		args = append(args, "-f", fmt.Sprintf("bestvideo[height=%d]+bestaudio/best", height))
	}
	args = append(args, "-S", "vcodec:h264,res,acodec:m4a", "--progress-template", "%(progress)j", "--newline", "--no-warnings", "-o", video.FileName, "-P", utils.DefaultDownloadDir, video.Url)

	cmd := exec.Command(utils.DefaultYtDlpCommand, args...)
	stdout, err := cmd.StdoutPipe()
	if err != nil {
		return err
	}

	if err := cmd.Start(); err != nil {
		return err
	}

	scanner := bufio.NewScanner(stdout)

	lastValidPercentage := 0
	lastUpdatedTime := time.Now()
	for scanner.Scan() {
		line := scanner.Text()

		var status utils.YtDlpDownloadInfo
		if err := json.Unmarshal([]byte(line), &status); err != nil {
			continue
		}

		if status.DownloadedBytes == 0 || status.TotalBytes == 0 || (status.DownloadedBytes >= status.TotalBytes) {
			continue
		}

		newPercent := int(math.Round((float64(status.DownloadedBytes) / float64(status.TotalBytes)) * 100))
		if newPercent <= video.DownloadPercent {
			continue
		}

		lastValidPercentage = newPercent

		currentTime := time.Now()
		elapsedTime := currentTime.Sub(lastUpdatedTime)

		if elapsedTime >= 1*time.Second && video.DownloadPercent < lastValidPercentage {
			lastUpdatedTime = currentTime
			video.DownloadPercent = lastValidPercentage
			database.DbConn.Save(video)
		}
	}

	if err := cmd.Wait(); err != nil {
		return err
	}

	video.DownloadPercent = 100
	database.DbConn.Save(video)
	return nil
}
