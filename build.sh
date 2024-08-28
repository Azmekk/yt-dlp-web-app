#!/bin/sh

export CGO_ENABLED=1

echo "Building go backend..."
cd ./src/backend

go mod download
go build -o ./bin/build/backend.exe

if [ ! -f ./bin/build/.env ]; then
    touch ./bin/build/.env
fi

echo "Done!"

echo "Building frontend..."
cd ../frontend
pnpm install
pnpm build

cd ../

if [ ! -d ./backend/bin/build/static ]; then
    rm -r ./backend/bin/build/static
    mkdir -p ./backend/bin/build/static
fi

cp -r ./frontend/build/* ./backend/bin/build/static/