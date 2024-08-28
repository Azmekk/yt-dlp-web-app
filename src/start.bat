@echo off

set "ffmpeg_url=https://github.com/yt-dlp/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest-win64-gpl.zip"
set "ytdlp_url=https://github.com/yt-dlp/yt-dlp/releases/download/2024.08.06/yt-dlp.exe"

set "ffmpeg_zip=ffmpeg-master-latest-win64-gpl.zip"
set "ffmpeg_dir=ffmpeg-master-latest-win64-gpl"
set "ffmpeg_exe=.\ffmpeg.exe"

set "ytdlp_exe=.\yt-dlp.exe"

if EXIST "%ffmpeg_exe%" (
    echo FFmpeg is already downloaded.
) else (
    echo Downloading FFmpeg ZIP...
   powershell -Command "$ProgressPreference = 'SilentlyContinue'; Invoke-WebRequest -Uri '%ffmpeg_url%' -OutFile '%ffmpeg_zip%'"

    echo Unzipping FFmpeg...
    powershell -Command "Expand-Archive -Path %ffmpeg_zip% -DestinationPath ."

    echo Copying FFmpeg binaries to current directory...
    xcopy "%ffmpeg_dir%\bin\*" "%cd%\" /y

    echo Cleaning up...
    del %ffmpeg_zip%
    rmdir /s /q "%ffmpeg_dir%"
)

if EXIST "%ytdlp_exe%" (
    echo yt-dlp.exe is already downloaded.
) else (
    echo Downloading yt-dlp...
    powershell -Command "$ProgressPreference = 'SilentlyContinue'; Invoke-WebRequest -Uri '%ytdlp_url%' -OutFile '%ytdlp_exe%'"
)

if exist "backend.exe" (
    echo Running backend.exe...
    start backend.exe
) else (
    echo backend.exe not found!
)

echo Done!