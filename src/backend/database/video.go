package database

import (
	"gorm.io/gorm"
)

type Video struct {
	gorm.Model
	FileName        string
	ThumnailName    string
	Size            int64
	Url             string `gorm:"uniqueIndex"`
	Downloaded      bool
	DownloadPercent int
}

const (
	SortBy_Date = iota
	SortBy_Title
	SortBy_Size
)

func GetExistingVideos(take int, offset int) (*[]Video, error) {
	var videos []Video

	result := DbConn.Order("created_at desc").Offset(offset).Limit(take).Find(&videos)
	if result.Error != nil {
		return nil, result.Error
	}

	return &videos, nil
}

func GetExistingVideosWithFilters(take int, offset int, orderBy int, descending bool, search string) (*[]Video, error) {
	var videos []Video

	orderColumn := getOrderByColumnName(orderBy)

	if descending {
		orderColumn += " desc"
	} else {
		orderColumn += " asc"
	}

	query := DbConn.Order(orderColumn)

	if search != "" {
		query = query.Where("file_name LIKE ?", "%"+search+"%")
	}

	query = query.Offset(offset).Limit(take)

	result := query.Find(&videos)
	if result.Error != nil {
		return nil, result.Error
	}

	return &videos, nil
}

func CreateVideo(url string, videoName string) (*Video, error) {
	newVideo := Video{FileName: videoName, ThumnailName: "", Size: 0, Url: url, Downloaded: false}
	result := DbConn.Create(&newVideo)
	if result.Error != nil {
		return nil, result.Error
	}

	return &newVideo, nil
}

func SaveDownloadedVideo(video *Video, filename string, thumbnailName string, size int64) error {
	video.FileName = filename
	video.ThumnailName = thumbnailName
	video.Size = size
	video.Downloaded = true

	result := DbConn.Save(video)
	if result.Error != nil {
		return result.Error
	}

	return nil
}

func GetVideoById(videoId uint) (*Video, error) {
	var queriedVideo Video
	result := DbConn.First(&queriedVideo, videoId)
	if result.Error != nil {
		return nil, result.Error
	}

	return &queriedVideo, nil
}

func DeleteVideoById(videoId uint) error {
	result := DbConn.Delete(&Video{}, videoId)
	if result.Error != nil {
		return result.Error
	}

	return nil
}

func GetVideosCount() (int64, error) {
	var videoCount int64
	result := DbConn.Model(&Video{}).Count(&videoCount)

	if result.Error != nil {
		return -1, result.Error
	}

	return videoCount, nil
}

func GetVideosCountWithFilters(orderBy int, descending bool, search string) (int64, error) {
	var videoCount int64

	orderColumn := getOrderByColumnName(orderBy)

	if descending {
		orderColumn += " desc"
	} else {
		orderColumn += " asc"
	}

	query := DbConn.Model(&Video{}).Order(orderColumn)

	if search != "" {
		query = query.Where("file_name LIKE ?", "%"+search+"%")
	}

	result := query.Count(&videoCount)

	if result.Error != nil {
		return -1, result.Error
	}

	return videoCount, nil
}

func CheckForExistingVideo(url string) (*Video, error) {
	var queriedVideo Video
	result := DbConn.Unscoped().Where("url", url).First(&queriedVideo)
	if result.Error != nil {
		return nil, result.Error
	}

	return &queriedVideo, nil
}

func RestoreDeletedVideo(video *Video) error {
	result := DbConn.Model(video).Update("DeletedAt", nil)
	if result.Error != nil {
		return result.Error
	}

	return nil
}

func getOrderByColumnName(orderBy int) string {
	var orderColumn string
	switch orderBy {
	case SortBy_Date:
		orderColumn = "created_at"
	case SortBy_Title:
		orderColumn = "file_name"
	case SortBy_Size:
		orderColumn = "size"
	default:
		orderColumn = "created_at"
	}

	return orderColumn
}
