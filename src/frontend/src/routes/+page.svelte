<script lang="ts">
	import Pagination from '$lib/components/Pagination/Pagination.svelte';
	import VideoDownload from '$lib/components/Videos/VideoDownload.svelte';
	import VideoView from '$lib/components/Videos/VideoView.svelte';
	import { orderByDescendingStore, orderByStore, videoSearchStore } from '$lib/Stores/FilterStores';
	import { usedStorageStore } from '$lib/Stores/UsedStorageStore';
	import { mdiCloseCircleOutline } from '@mdi/js';
	import { onDestroy, onMount } from 'svelte';
	import { Icon, Notification, ProgressCircle } from 'svelte-ux';
	import { type Video } from '$lib/api-clients/backend-client/models/Video';
	import { VideoOrderBy } from '$lib/index';
	import {
		StorageApi,
		VideosApi,
		type ApiVideosListVideosGetRequest
	} from '$lib/api-clients/backend-client/apis/index';
	import { type VideoCountResponse } from '$lib/api-clients/backend-client/models';
	import { WebSocketService, type VideoDownloadInfo } from '$lib/websocketService';
	import { BASE_PATH, BaseAPI } from '$lib/api-clients/backend-client';

	interface VodeoDownloadInfo {
		videoId: number;
		downloadPercent: number;
	}
	let videosInProgress: VodeoDownloadInfo[] = [];

	let videos: Video[] = [];
	let videoCount = 0;
	let videosPerPage = 3;
	let currentPage = 1;
	let totalPages = 0;

	let innerWidth = 0;
	let innerHeight = 0;

	let errorTitle = '';
	let errorMessage = '';
	let errorDialogVisible = false;

	function hideErrorDialog() {
		errorTitle = '';
		errorMessage = '';
		errorDialogVisible = false;
	}

	function showErrorDialog(title: string, error: string) {
		errorTitle = title;
		errorMessage = error;
		errorDialogVisible = true;

		setTimeout(() => {
			hideErrorDialog();
		}, 5000);
	}

	let orderByFilter: VideoOrderBy = VideoOrderBy.CreationDate;
	let orderByDescending: boolean = true;
	let videoSearch: string = '';
	async function getVideosAsync() {
		videosInProgress = [];
		let videosApi = new VideosApi();
		let videosResult: Video[] | null = null;
		let totalVideosAmountResult: VideoCountResponse | null = null;

		try {
			const request: ApiVideosListVideosGetRequest = {
				take: videosPerPage,
				page: currentPage,
				orderBy: orderByFilter,
				descending: orderByDescending,
				search: videoSearch
			};
			videosResult = await videosApi.apiVideosListVideosGet(request);
			totalVideosAmountResult = await videosApi.apiVideosGetVideoCountGet({ search: videoSearch });

			videosResult.forEach(async (x) => {
				if (!x.downloaded && x.id) {
					var downloadInfo = await videosApi.apiVideosGetVideoDownloadInfoGet({ videoId: x.id });
					videosInProgress = [
						...videosInProgress,
						{ videoId: x.id, downloadPercent: downloadInfo.downloadPercent ?? 0 }
					];
				}
			});
		} catch (error) {
			alert('Failed to fetch videos.');
			console.log('Failed to fetch videos. Error: ' + error);
			showErrorDialog('Fetch error', 'Failed to fetch videos.');
		} finally {
			videos = videosResult ?? [];
			videoCount = totalVideosAmountResult?.count ?? 0;
		}
	}

	async function updateUsedStorageAsync() {
		let storageApi = new StorageApi();
		try {
			let response = await storageApi.apiStorageGetUsedStorageGet();
			usedStorageStore.update((x) => (x = response.usedStorage ?? 0));
		} catch (error) {
			alert('Something went wrong when fetching storage usage.');
			console.error('Something went wrong when fetching storage usage: ', error);
		}
	}

	let isLoading = true;
	async function updateVideoInfoAsync() {
		isLoading = true;
		await new Promise((resolve) => setTimeout(resolve, 100));

		videosPerPage = Math.max(Math.floor(Math.floor((innerWidth - 240) / 480) * 2), 3);

		await getVideosAsync();

		totalPages = Math.ceil(videoCount / videosPerPage);

		await updateUsedStorageAsync();
		isLoading = false;
	}

	orderByStore.subscribe(async (x) => {
		if (orderByFilter != x) {
			orderByFilter = x;
			await updateVideoInfoAsync();
		}
	});

	orderByDescendingStore.subscribe(async (x) => {
		if (orderByDescending != x) {
			orderByDescending = x;
			await updateVideoInfoAsync();
		}
	});

	let searchTimeout: NodeJS.Timeout;
	videoSearchStore.subscribe(async (x) => {
		if (videoSearch != x) {
			videoSearch = x;

			if (searchTimeout) {
				clearTimeout(searchTimeout);
			}
			searchTimeout = setTimeout(async () => {
				currentPage = 1;
				await updateVideoInfoAsync();
			}, 400);
		}
	});

	let interval: NodeJS.Timeout;
	let webSocketService: WebSocketService;

	async function handleIncomingSocketMessage(message: string): Promise<void> {
		let mediaDownloadInfo: any = JSON.parse(message);

		if ('downloadPercent' in mediaDownloadInfo) {
			if (mediaDownloadInfo.downloaded) {
				await updateVideoInfoAsync();
				return;
			}

			videosInProgress.forEach((x) => {
				if (x.videoId == mediaDownloadInfo.videoId) {
					x.downloadPercent = mediaDownloadInfo.downloadPercent;
				}
			});
		}
		else if ('converted' in mediaDownloadInfo) {
			await updateVideoInfoAsync();
		}

		videosInProgress = [...videosInProgress];
	}

	function renewSocket(): void {
		webSocketService.sendMessage('renew');
	}

	function getWsPath(): string {
		if (BASE_PATH == '') {
			return `${window.location.protocol == 'https:' ? 'wss' : 'ws'}://${window.location.hostname + ':' + window.location.port + '/ws'}`;
		} else {
			let baseUrl = new URL(BASE_PATH);
			return `${baseUrl.protocol == 'https:' ? 'wss' : 'ws'}://${baseUrl.hostname + ':' + baseUrl.port + '/ws'}`;
		}
	}

	onMount(async () => {
		await updateVideoInfoAsync();

		webSocketService = new WebSocketService(getWsPath(), handleIncomingSocketMessage);
		webSocketService.connect();
		location;

		interval = setInterval(renewSocket, 10000);
	});

	onDestroy(async () => {});
</script>

<svelte:window bind:innerWidth bind:innerHeight />

<main style="height: {innerHeight - 64}px" class="p-2 pl-6 pr-6">
	{#if isLoading}
		<div class="flex w-full pt-10 justify-center items-center">
			<ProgressCircle size={128} width={5} class="text-black dark:text-white" />
		</div>
	{:else}
		<div class=" flex flex-col h-full">
			<VideoDownload
				on:videoSaved={async () => await updateVideoInfoAsync()}
				on:videoSaveFail={(e) => {
					showErrorDialog('Error', `Failed to download video: ${e.detail.error}`);
				}}
			/>

			<div class="flex w-full justify-center items-center">
				<div
					style="width: {480 > innerWidth ? innerWidth : (videosPerPage * 500) / 2}px"
					class="flex flex-wrap justify-left mt-5"
				>
					{#if videos.length < 1}
						<p class="text-xl font-medium w-full h-full text-center">
							No videos. Start by downloading some!
						</p>
					{/if}
					{#each videos as video (video.id)}
						<VideoView
							on:videoDeleted={async () => await updateVideoInfoAsync()}
							bind:video
							downloadPercent={videosInProgress.find((x) => x.videoId === video.id)
								?.downloadPercent ?? 1}
						/>
					{/each}
				</div>
			</div>

			<div class="mb-2 mt-auto flex justify-center">
				<Pagination
					on:pageUpdated={updateVideoInfoAsync}
					bind:currentPage
					bind:total={videoCount}
					bind:perPage={videosPerPage}
				/>
			</div>
		</div>

		<div class="fixed bottom-0 right-0 z-5 mb-5 mr-5">
			<div class="w-[400px]">
				<Notification open={errorDialogVisible} closeIcon>
					<div slot="icon">
						<Icon data={mdiCloseCircleOutline} class="text-red-500" />
					</div>
					<div slot="title">{errorTitle}</div>
					<div slot="description">{errorMessage}</div>
				</Notification>
			</div>
		</div>
	{/if}
</main>
