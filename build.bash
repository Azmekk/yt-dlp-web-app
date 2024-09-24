#!/bin/bash

if [ "$#" -ne 1 ]; then
    echo "Usage: $0 <path_to_7z>"
    exit 1
fi

SEVEN_ZIP="$1"
APP_NAME="YT-DLP-Web-App-Backend" 
OUTPUT_DIR="./bin/github-release"

echo "Building backend..."
cd ./src/backend/YT-DLP-Web-App-Backend

dotnet restore

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

echo "Compressing..."
"$SEVEN_ZIP" a -tzip "$OUTPUT_DIR/linux-x64.zip" "$OUTPUT_DIR/linux-x64/*" -mx=9
"$SEVEN_ZIP" a -tzip "$OUTPUT_DIR/linux-arm64.zip" "$OUTPUT_DIR/linux-arm64/*" -mx=9
"$SEVEN_ZIP" a -tzip "$OUTPUT_DIR/win-x64.zip" "$OUTPUT_DIR/win-x64/*" -mx=9
"$SEVEN_ZIP" a -tzip "$OUTPUT_DIR/win-arm64.zip" "$OUTPUT_DIR/win-arm64/*" -mx=9
echo "Done!"