#!/bin/sh

if [ "$#" -ne 1 ]; then
    echo "Usage: $0 <path_to_7z>"
    exit 1
fi

SEVEN_ZIP="$1"
APP_NAME="YT-DLP-Web-App-Backend" 
OUTPUT_DIR="./bin/tauri-release"

echo "Building backend..."
cd ../backend/YT-DLP-Web-App-Backend

dotnet restore

dotnet publish -c Release -r win-x64 --self-contained true -o "$OUTPUT_DIR/win-x64" -p:PublishSingleFile=true "$APP_NAME.csproj" || exit 1

echo "Done!"

echo Downloading and making dependencies:

mkdir "$OUTPUT_DIR/win-x64/Downloads"
mkdir "$OUTPUT_DIR/win-x64/Database"

FFMPEG_DOWNLOAD_PATH="$OUTPUT_DIR/win-x64/ffmpeg_bundle.zip"

curl -L -o "$OUTPUT_DIR/win-x64/yt-dlp.exe" https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp.exe
curl -L -o "$FFMPEG_DOWNLOAD_PATH" https://github.com/yt-dlp/FFmpeg-Builds/releases/latest/download/ffmpeg-master-latest-win64-gpl.zip

FFMPEG_UNZIP_PATH="$OUTPUT_DIR/win-x64/ffmeg_bundle"
"$SEVEN_ZIP" e -o"$FFMPEG_UNZIP_PATH" "$FFMPEG_DOWNLOAD_PATH" -y

mv "$FFMPEG_UNZIP_PATH/ffmpeg.exe" $OUTPUT_DIR/win-x64/ffmpeg.exe
mv "$FFMPEG_UNZIP_PATH/ffprobe.exe" $OUTPUT_DIR/win-x64/ffprobe.exe

rm "$FFMPEG_DOWNLOAD_PATH"
rm -r "$FFMPEG_UNZIP_PATH"

echo "Building tauri app..."

cd ../../frontend

pnpm tauri build