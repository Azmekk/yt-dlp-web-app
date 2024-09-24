#!/bin/bash

echo "Building backend..."
cd ./src/backend/YT-DLP-Web-App-Backend

APP_NAME="YT-DLP-Web-App-Backend" 
OUTPUT_DIR="./bin/github-release"

RUN dotnet restore

dotnet publish -c Release -r win-x64 --self-contained true -o "$OUTPUT_DIR/win-x64" -p:PublishSingleFile=true "$APP_NAME.csproj" || exit 1
dotnet publish -c Release -r linux-x64 --self-contained true -o "$OUTPUT_DIR/linux-x64" -p:PublishSingleFile=true "$APP_NAME.csproj" || exit 1
dotnet publish -c Release -r win-arm64 --self-contained true -o "$OUTPUT_DIR/windows-arm64" -p:PublishSingleFile=true "$APP_NAME.csproj" || exit 1
dotnet publish -c Release -r linux-arm64 --self-contained true -o "$OUTPUT_DIR/linux-arm64" -p:PublishSingleFile=true "$APP_NAME.csproj" || exit 1

echo "Done!"

echo "Building frontend..."
cd ../../frontend
pnpm install
pnpm build

cd ../backend/YT-DLP-Web-App-Backend

for arch in win-x64 linux-x64 win-arm64 linux-arm64; do
    if [ ! -d "$OUTPUT_DIR/$arch/Static" ]; then
        mkdir -p "$OUTPUT_DIR/$arch/Static"
    fi
done

echo "Copying files..."

cp -r ../../frontend/build/* "$OUTPUT_DIR/win-x64/Static"
cp -r ../../frontend/build/* "$OUTPUT_DIR/linux-x64/Static"
cp -r ../../frontend/build/* "$OUTPUT_DIR/win-arm64/Static"
cp -r ../../frontend/build/* "$OUTPUT_DIR/linux-arm64/Static"

echo "Done!"
