FROM node:22.7.0-alpine AS yt-dlp-web-frontend-builder

WORKDIR /app/frontend

COPY ./src/frontend ./

RUN apk update
RUN apk upgrade
RUN apk add --no-cache curl

RUN curl -L https://unpkg.com/@pnpm/self-installer | node

RUN pnpm install
RUN pnpm build

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS yt-dlp-web-backend-builder

WORKDIR /app/backend

COPY ./src/backend/YT-DLP-Web-App-Backend ./

RUN dotnet restore
RUN dotnet publish -c Release -r linux-x64 -o "./bin/docker-release/linux-x64" "YT-DLP-Web-App-Backend.csproj"

FROM mcr.microsoft.com/dotnet/runtime:9.0

WORKDIR /app/yt-dlp-web

COPY --from=yt-dlp-web-frontend-builder /app/frontend/build /app/yt-dlp-web/backend/Static
COPY --from=yt-dlp-web-backend-builder /app/backend/bin/docker-release/linux-x64 /app/yt-dlp-web/backend

RUN chmod +x /app/yt-dlp-web/backend/YT-DLP-Web-App-Backend
RUN apt-get update
RUN apt-get upgrade -y

EXPOSE 41001

CMD ["/app/yt-dlp-web/backend/YT-DLP-Web-App-Backend"]