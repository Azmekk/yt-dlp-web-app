#!/bin/bash

echo "Building backend..."
cd ./src/backend/YT-DLP-Web-App-Backend

APP_NAME="YT-DLP-Web-App-Backend" 
OUTPUT_DIR="./bin/github-release"

dotnet publish -c Release -r win-x64 --self-contained true -o "$OUTPUT_DIR/win-x64" -p:PublishSingleFile=true "$APP_NAME.csproj"
dotnet publish -c Release -r linux-x64 --self-contained true -o "$OUTPUT_DIR/linux-x64" -p:PublishSingleFile=true "$APP_NAME.csproj"
dotnet publish -c Release -r windows-arm64 --self-contained true -o "$OUTPUT_DIR/windows-arm64" -p:PublishSingleFile=true "$APP_NAME.csproj"
dotnet publish -c Release -r linux-arm64 --self-contained true -o "$OUTPUT_DIR/linux-arm64" -p:PublishSingleFile=true "$APP_NAME.csproj"

echo "Done!"

echo "Building frontend..."
cd ../../frontend
pnpm install
pnpm build

cd ../backend/YT-DLP-Web-App-Backend

if [ ! -d "$OUTPUT_DIR/win-x64/Static"]; then
    rm -r "$OUTPUT_DIR/win-x64/Static"
    mkdir -p "$OUTPUT_DIR/win-x64/Static"
fi

if [ ! -d "$OUTPUT_DIR/linux-x64/Static"]; then
    rm -r "$OUTPUT_DIR/linux-x64/Static"
    mkdir -p "$OUTPUT_DIR/linux-x64/Static"
fi

if [ ! -d "$OUTPUT_DIR/windows-arm64/Static"]; then
    rm -r "$OUTPUT_DIR/windows-arm64/Static"
    mkdir -p "$OUTPUT_DIR/windows-arm64/Static"
fi

if [ ! -d "$OUTPUT_DIR/linux-arm64/Static"]; then
    rm -r "$OUTPUT_DIR/linux-arm64/Static"
    mkdir -p "$OUTPUT_DIR/linux-arm64/Static"
fi

echo "Copying files..."

cp -r ../../frontend/build/* "$OUTPUT_DIR/win-x64/Static"
cp -r ../../frontend/build/* "$OUTPUT_DIR/linux-x64/Static"
cp -r ../../frontend/build/* "$OUTPUT_DIR/windows-arm64/Static"
cp -r ../../frontend/build/* "$OUTPUT_DIR/linux-arm64/Static"

echo "Done!"