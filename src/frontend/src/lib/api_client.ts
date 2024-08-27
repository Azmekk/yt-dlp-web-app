export type ApiVideoListResponse = {
    videos: ApiVideoResponse[];
    totalAmount: number
}

export type ApiVideoResponse = {
    id: number;
    creation_time: string;
    name: string;
    thumbnailName: string;
    size: number;
    url: string;
    downloaded: boolean
    downloadPercent: number
}

export type ApiVideoCountResponse = {
    count: number;
}

export type TitleResponse = {
    title: string;
}

export type VideoDimensionsResponse = {
    width: number;
    height: number;
}

export type ErrorResponse = {
    error: string;
}

export type DriveUsageResponse = {
    usedStorage: number;
}

export enum VideoOrderByParam{
    Date,
    Title,
    Size
}

export async function saveVideo(videoUrl: string, videoName: string, mediaFormat: string, height: number = 0): Promise<ApiVideoResponse | null> {
    let path = "/api/videos/save"
    var url = new URL(path, window.location.href);
    url.searchParams.append("video", videoUrl);
    url.searchParams.append("name", videoName);
    url.searchParams.append("format", mediaFormat);

    if (height != 0) {
        url.searchParams.append("resolution", height.toString() + "p");
    }
    

    const response = await fetchWithCors(url.toString());

    if (response.status == 409) {
        const data: ErrorResponse = await response.json();
        throw new Error(data.error)
    }

    if (!response.ok) {
        throw new Error(`${response.statusText}`);
    }

    const data: ApiVideoResponse = await response.json();

    return data;
}

export async function getYoutubeName(videoUrl: string): Promise<TitleResponse | null> {
    let path = "/api/videos/getYoutubeName"
    var url = new URL(path, window.location.href);
    url.searchParams.append("video", videoUrl);

    const response = await fetchWithCors(url.toString());

    if (!response.ok) {
        throw new Error(`${response.statusText}`);
    }

    const data: TitleResponse = await response.json();

    return data;
}



export async function fetchVideosJson(take: number, page: number, orderBy: VideoOrderByParam = 0, descending: boolean = true, search: string = "" ): Promise<ApiVideoListResponse | null> {
    let path = "/api/videos/list"
    var url = new URL(path, window.location.href);
    url.searchParams.append("take", take.toString());
    url.searchParams.append("page", page.toString());
    url.searchParams.append("orderBy", orderBy.toString());
    url.searchParams.append("descending", descending.toString());
    url.searchParams.append("search", search.toString());

    const response = await fetchWithCors(url.toString());

    if (!response.ok) {
        throw new Error(`${response.statusText}`);
    }

    const data: ApiVideoListResponse = await response.json();

    return data;
}
export async function fetchVideoCount(): Promise<ApiVideoCountResponse | null> {
    let path = "/api/videos/getcount"
    var url = new URL(path, window.location.href);
    const response = await fetchWithCors(url.toString());

    if (!response.ok) {
        throw new Error(`${response.statusText}`);
    }

    const data: ApiVideoCountResponse = await response.json();

    return data;
}

export async function fetchVideoInfo(videoId: number): Promise<ApiVideoResponse | null> {
    let path = "/api/videos/getInfo"
    var url = new URL(path, window.location.href);
    url.searchParams.append("id", videoId.toString());

    const response = await fetchWithCors(url.toString());

    if (!response.ok) {
        throw new Error(`${response.statusText}`);
    }

    const data: ApiVideoResponse = await response.json();

    return data;
}

export async function deleteVideo(videoId: number) {
    let path = "/api/videos/delete"
    var url = new URL(path, window.location.href);
    url.searchParams.append("id", videoId.toString());

    const response = await fetchWithCors(url.toString());

    if (!response.ok) {
        throw new Error(`${response.statusText}`);
    }
}

export async function renameVideo(videoId: number, newName: string) {
    let path = "/api/videos/rename"
    var url = new URL(path, window.location.href);
    url.searchParams.append("id", videoId.toString());
    url.searchParams.append("newName", newName);

    const response = await fetchWithCors(url.toString());

    if (!response.ok) {
        throw new Error(`${response.statusText}`);
    }
}

export async function getVideoDimensions(videoUrl: string): Promise<VideoDimensionsResponse> {
    let path = "/api/videos/getVideoDimensions"
    var url = new URL(path, window.location.href);
    url.searchParams.append("video", videoUrl);

    const response = await fetchWithCors(url.toString());


    if (!response.ok) {
        throw new Error(`${response.statusText}`);
    }

    const data: VideoDimensionsResponse = await response.json();

    return data;
}

export async function getUsedStorage(): Promise<DriveUsageResponse>{
    let path = "/api/videos/getStorageInfo"
    var url = new URL(path, window.location.href);

    const response = await fetchWithCors(url.toString());

    if (!response.ok) {
        throw new Error(`${response.statusText}`);
    }

    const data: DriveUsageResponse = await response.json();

    return data;
}

export function getDownloadVideoPath(videoId: number): string {
    let path = "/api/videos/download"
    var url = new URL(path, window.location.href);
    url.searchParams.append("id", videoId.toString());

    return url.toString();
}

export function getMp3DownloadPath(videoId: number): string {
    let path = "/api/videos/downloadMp3"
    var url = new URL(path, window.location.href);
    url.searchParams.append("id", videoId.toString());

    return url.toString();
}

export function getVideoPath(videoId: number): string {
    let path = "/api/videos/video"
    var url = new URL(path, window.location.href);
    url.searchParams.append("id", videoId.toString());

    return url.toString();
}

export function getThumbnailPath(videoId: number): string {
    let path = "/api/videos/thumbnail"
    var url = new URL(path, window.location.href);
    url.searchParams.append("id", videoId.toString());

    return url.toString();
}

export function fetchWithCors(path: string): Promise<Response> {
    return fetch(path, {
        method: "get",
        mode: "cors",
        credentials: "include",
        headers: {
            "Content-Type": "application/json"
        },
    })
}