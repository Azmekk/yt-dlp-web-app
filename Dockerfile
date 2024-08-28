FROM node:20.17.0-alpine AS yt-dlp-web-frontend-builder

WORKDIR /app/frontend

COPY ./src/frontend ./

RUN apk update
RUN apk upgrade
RUN apk add --no-cache curl

RUN curl -L https://unpkg.com/@pnpm/self-installer | node

RUN pnpm install
RUN pnpm build

FROM golang:1.23.0-alpine AS yt-dlp-web-backend-builder

WORKDIR /app/backend

COPY ./src/backend ./

RUN apk add build-base

RUN go mod download
RUN CGO_ENABLED=1 GOOS=linux go build -o ./bin/build/backend

FROM alpine:latest

WORKDIR /app/yt-dlp-web

COPY --from=yt-dlp-web-frontend-builder /app/frontend/build ./static
COPY --from=yt-dlp-web-backend-builder /app/backend/bin/build/backend ./backend

RUN apk update
RUN apk upgrade
RUN apk add --no-cache ffmpeg
RUN apk add --no-cache yt-dlp

EXPOSE 41001

CMD ["./backend"] 