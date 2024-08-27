package utils

import (
	"encoding/json"
	"fmt"
	"io"
	"io/fs"
	"math/rand"
	"net/http"
	"os"
	"os/exec"
	"path/filepath"
	"runtime"
	"strings"
	"time"

	"github.com/joho/godotenv"
)

type ErrorResponse struct {
	Error string `json:"error"`
}

var Port string = "41001"

var DefaultDownloadDir string = "./downloads"
var DefaultDatabaseDir string = "./database"
var DefaultStaticDir string = "./static"

var Rand *rand.Rand

var AllowedFileCharsRegex = `[^A-Za-z0-9-_]+`

var allowedRandomChars = []byte("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")

var innerClient = &http.Client{Timeout: 10 * time.Second}

func InitiateRand() {
	Rand = rand.New(rand.NewSource(time.Now().UnixNano()))
}

func AdjustDefaultDirsBasedOnOs() {
	if runtime.GOOS == "windows" {
		DefaultDownloadDir = ".\\downloads"
		DefaultDatabaseDir = ".\\database"
		DefaultStaticDir = ".\\static"
	}
}

func ReadAndSetEnvVars() error {
	if !FileExists(".env") {
		return nil
	}

	envFile, err := godotenv.Read(".env")
	if err != nil {
		return err
	}

	if envFile["DEFAULT_DOWNLOAD_DIR"] != "" {
		downloadDirStr := envFile["DEFAULT_DOWNLOAD_DIR"]
		fmt.Printf("Setting download dir to: %s\n", downloadDirStr)
		DefaultDownloadDir = downloadDirStr
	}

	if envFile["DEFAULT_STATIC_DIR"] != "" {
		defaultStaticDir := envFile["DEFAULT_STATIC_DIR"]
		fmt.Printf("Setting static dir to: %s\n", defaultStaticDir)
		DefaultStaticDir = defaultStaticDir
	}

	if envFile["DEFAULT_DATABASE_DIR"] != "" {
		databaseDirStr := envFile["DEFAULT_DATABASE_DIR"]
		fmt.Printf("Setting database dir to: %s\n", databaseDirStr)
		DefaultDatabaseDir = databaseDirStr
	}

	return nil
}

func RandomString(length int) string {
	b := make([]byte, length)
	for i := range b {
		b[i] = allowedRandomChars[Rand.Intn(len(allowedRandomChars))]
	}
	return string(b)
}

func FileExists(path string) bool {
	_, err := os.Stat(path)
	if err != nil {
		if os.IsNotExist(err) {
			return false
		}
	}
	return true
}

func DirExists(path string) (bool, error) {
	_, err := os.Stat(path)

	if os.IsNotExist(err) {
		return false, nil
	}

	if err != nil {
		return false, err
	}

	return true, nil
}

func FindVideoImage(videoName string) (string, error) {
	var file string
	err := filepath.WalkDir(DefaultDownloadDir, func(path string, d fs.DirEntry, err error) error {
		if d.IsDir() {
			return nil
		}

		baseFileName := filepath.Base(path)
		if strings.HasPrefix(baseFileName, videoName) && filepath.Ext(baseFileName) != ".mp4" {
			file = baseFileName
			return nil
		}

		return nil
	})

	return file, err
}

func FetchJson(url string, target interface{}) error {
	r, err := innerClient.Get(url)
	if err != nil {
		return err
	}
	defer r.Body.Close()

	return json.NewDecoder(r.Body).Decode(target)
}

func DownloadAndCreateThumbnail(url string, videoName string) string {
	if url == "" {
		return ""
	}

	thumbnailExt := ""

	thumbnailExt = filepath.Ext(url)
	fileName := fmt.Sprintf("%s.%s", videoName, thumbnailExt)

	thumbnailFile, err := os.Create(filepath.Join(DefaultDownloadDir, fileName))
	if err != nil {
		fmt.Printf("Failed to create file due to err: %s\n", err)
		return ""
	}
	defer thumbnailFile.Close()

	response, err := http.Get(url)
	if err != nil {
		fmt.Printf("Failed to download file due to err: %s\n", err)
		return ""
	}
	defer response.Body.Close()

	_, err = io.Copy(thumbnailFile, response.Body)
	if err != nil {
		fmt.Printf("Failed to write response to file due to err: %s\n", err)
		return ""
	}

	return fileName
}

func CreateThumbnailFromFrame(videoName string) string {
	videoPath := filepath.Join(DefaultDownloadDir, videoName)
	if _, err := os.Stat(videoPath); os.IsNotExist(err) {
		fmt.Printf("video file does not exist: %s\n", videoPath)
		return ""
	}

	outputPath := filepath.Join(DefaultDownloadDir, fmt.Sprintf("%s_thumbnail.jpg", videoName))

	cmd := exec.Command(DefaultFfmpegCommand, "-i", videoPath, "-vf", "select=eq(n\\,0)", "-vsync", "vfr", "-q:v", "2", outputPath)
	if err := cmd.Run(); err != nil {
		fmt.Printf("failed to extract frame: %v\n", err)
		return ""
	}

	return outputPath
}

func GetFolderSize(folderPath string) (int64, error) {
	var size int64

	err := filepath.Walk(folderPath, func(_ string, info os.FileInfo, err error) error {
		if err != nil {
			return err
		}
		if !info.IsDir() {
			size += info.Size()
		}
		return nil
	})

	return size, err
}

func GetUsedFoldersSize() (int64, error) {
	downloadDirSize, err := GetFolderSize(DefaultDownloadDir)
	if err != nil {
		return 0, err
	}

	dbDirSize, err := GetFolderSize(DefaultDatabaseDir)
	if err != nil {
		return 0, err
	}

	return downloadDirSize + dbDirSize, nil

}

func GetFilenameWithoutExt(fileName string) string {
	extension := filepath.Ext(fileName)
	return fileName[0 : len(fileName)-len(extension)]
}
