<script lang="ts">
	import {
		fetchVideoInfo,
		fetchVideosJson,
		getUsedStorage,
		VideoOrderByParam,
		type ApiVideoResponse
	} from '$lib/api_client';
	import Pagination from '$lib/components/Pagination/Pagination.svelte';
	import VideoDownload from '$lib/components/Videos/VideoDownload.svelte';
	import VideoView from '$lib/components/Videos/VideoView.svelte';
	import { orderByDescendingStore, orderByStore, videoSearchStore } from '$lib/Stores/FilterStores';
	import { usedStorageStore } from '$lib/Stores/UsedStorageStore';
	import { mdiCloseCircleOutline } from '@mdi/js';
	import { onMount } from 'svelte';
	import { Icon, Notification, ProgressCircle } from 'svelte-ux';

	let videos: ApiVideoResponse[] = [];
	let videoCount = 0;
	let videosPerPage = 3;
	let page = 1;
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

	let orderBy: VideoOrderByParam = VideoOrderByParam.Date;
	let orderByDescending: boolean = true;
	let videoSearch: string = '';
	async function getVideos() {
		let result = null;
		try {
			result = await fetchVideosJson(videosPerPage, page, orderBy, orderByDescending, videoSearch);
		} catch (error) {
			console.log('Failed to fetch videos. Error: ' + error);
			showErrorDialog('Fetch error', 'Failed to fetch videos.');
		} finally {
			videos = result?.videos ?? [];
			videoCount = result?.totalAmount ?? 0;
		}
	}

	async function updateUsedStorage() {
		try {
			let response = await getUsedStorage();
			usedStorageStore.update((x) => (x = response.usedStorage));
		} catch (error) {
			console.error('Something went wrong when fetching storage usage: ', error);
		}
	}

	let isLoading = true;
	async function updateVideoInfo() {
		isLoading = true;
		await new Promise((resolve) => setTimeout(resolve, 100));

		videosPerPage = Math.max(Math.floor(Math.floor((innerWidth - 240) / 480) * 2), 3);

		await getVideos();

		totalPages = Math.ceil(videoCount / videosPerPage);

		await updateUsedStorage();
		isLoading = false;
	}

	orderByStore.subscribe(async (x) => {
		if (orderBy != x) {
			orderBy = x;
			await updateVideoInfo();
		}
	});

	orderByDescendingStore.subscribe(async (x) => {
		if (orderByDescending != x) {
			orderByDescending = x;
			await updateVideoInfo();
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
				page = 1;
				await updateVideoInfo();
			}, 400);
		}
	});

	let interval: NodeJS.Timeout;
	async function checkVideosDownloadStatus() {
		if (videos.every((video) => video.downloaded)) {
			return;
		}

		for (let i = 0; i < videos.length; i++) {
			const video = videos[i];

			if (video.downloaded) {
				continue;
			}

			try {
				const updatedVideo = await fetchVideoInfo(video.id);
				if (updatedVideo == null) {
					throw new Error('Video response was null.');
				}
				videos[i] = updatedVideo;
				videos = videos;
			} catch (error) {
				console.error('Failed to fetch video information: ', error);
			}
		}
	}

	onMount(async () => {
		await updateVideoInfo();
		interval = setInterval(checkVideosDownloadStatus, 1000);
	});
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
				on:videoSaved={async () => await updateVideoInfo()}
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
						<VideoView on:videoDeleted={async () => await updateVideoInfo()} bind:video />
					{/each}
				</div>
			</div>

			<div class="mb-2 mt-auto flex justify-center">
				<Pagination
					on:pageUpdated={updateVideoInfo}
					bind:currentPage={page}
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
