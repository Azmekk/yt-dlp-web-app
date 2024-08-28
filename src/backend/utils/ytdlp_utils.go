package utils

import (
	"fmt"
	"os"
	"os/exec"
	"runtime"
	"strconv"
	"strings"
)

type YtDlpDownloadInfo struct {
	//Status                string  `json:"status"`
	DownloadedBytes int `json:"downloaded_bytes"`
	TotalBytes      int `json:"total_bytes"`
	//FragmentIndex         int     `json:"fragment_index"`
	//FragmentCount         int     `json:"fragment_count"`
	//Filename              string  `json:"filename"`
	//Tmpfilename           string  `json:"tmpfilename"`
	//MaxProgress           int     `json:"max_progress"`
	//ProgressIdx           int     `json:"progress_idx"`
	//Elapsed               float64 `json:"elapsed"`
	//TotalBytesEstimate    float64 `json:"total_bytes_estimate"`
	//Speed                 int     `json:"speed"`
	//Eta                   int     `json:"eta"`
	//EtaStr                string  `json:"_eta_str"`
	//SpeedStr              string  `json:"_speed_str"`
	PercentStr string `json:"_percent_str"`
	//TotalBytesStr         string  `json:"_total_bytes_str"`
	//TotalBytesEstimateStr string  `json:"_total_bytes_estimate_str"`
	//DownloadedBytesStr    string  `json:"_downloaded_bytes_str"`
	//ElapsedStr            string  `json:"_elapsed_str"`
	//DefaultTemplate       string  `json:"_default_template"`
}

var DefaultYtDlpCommand string = "yt-dlp"
var DefaultFfmpegCommand string = "ffmpeg"
var DefaultFfProbeCommand string = "ffprobe"

func CheckDependencies() error {
	if checkPathForDependencies() {
		fmt.Println("Found all dependencies on path.")
		return nil
	}

	if checkDirForDependencies() {
		fmt.Println("Found all dependencies in local folder and updated commands.")
		return nil
	}

	return fmt.Errorf("one or more dependencies were not found")
}

func checkPathForDependencies() bool {
	if runtime.GOOS == "windows" {
		DefaultYtDlpCommand += ".exe"
		DefaultFfmpegCommand += ".exe"
		DefaultFfProbeCommand += ".exe"
	}

	commands := []string{DefaultYtDlpCommand, DefaultFfmpegCommand, DefaultFfProbeCommand}
	for _, command := range commands {
		path, err := exec.LookPath(command)
		if err != nil || path == "" {
			fmt.Printf("Failed to locate %s on path. Checking local dir for dependencies.\n", command)
			return false
		}
	}

	return true
}

func checkDirForDependencies() bool {
	if runtime.GOOS == "windows" {
		DefaultYtDlpCommand = ".\\" + DefaultYtDlpCommand
		DefaultFfmpegCommand = ".\\" + DefaultFfmpegCommand
		DefaultFfProbeCommand = ".\\" + DefaultFfProbeCommand
	} else {
		DefaultYtDlpCommand = "./" + DefaultYtDlpCommand
		DefaultFfmpegCommand = "./" + DefaultFfmpegCommand
		DefaultFfProbeCommand = "./" + DefaultFfProbeCommand
	}

	ytDlpExsts, err := checkFileExists(DefaultYtDlpCommand)
	if err != nil || !ytDlpExsts {
		fmt.Println("Failed to locate yt-dlp local file.", err)
		return false
	}

	ffmpegExists, err := checkFileExists(DefaultFfmpegCommand)
	if err != nil || !ffmpegExists {
		fmt.Println("Failed to locate ffmpeg local file.", err)
		return false
	}

	ffprobeExists, err := checkFileExists(DefaultFfProbeCommand)
	if err != nil || !ffprobeExists {
		fmt.Println("Failed to locate ffprobe local file.", err)
		return false
	}

	return true
}

func checkFileExists(filePath string) (bool, error) {
	_, err := os.Stat(filePath)
	if err != nil {
		if os.IsNotExist(err) {
			return false, nil
		}
		return false, err
	}

	return true, nil
}

func GetMaxVideoResolution(url string) (int, int) {
	cmd := exec.Command(DefaultYtDlpCommand, "--print", "width", "--print", "height", "--skip-download", "-q", "--no-warnings", url)

	commandOutput, err := cmd.CombinedOutput()
	if err != nil {
		fmt.Println("Error when trying to read resolution output: ", err)
		return 0, 0
	}

	stringOutput := string(commandOutput)

	lines := strings.Split(stringOutput, "\n")

	width, err := strconv.Atoi(lines[0])
	if err != nil {
		fmt.Println("Error when parsing width output: ", err)
		return 0, 0
	}

	height, err := strconv.Atoi(lines[1])
	if err != nil {
		fmt.Println("Error when parsing height output: ", err)
		return 0, 0
	}

	return width, height
}
