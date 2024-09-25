<script lang="ts">
	import { BASE_PATH, Mp3Status, VideosApi, type Video } from '$lib/api-clients/backend-client';
	import { formatBytes, formatDate } from '$lib/utils';
	import {
		mdiCancel,
		mdiCheck,
		mdiDelete,
		mdiDownload,
		mdiPencil,
		mdiTrashCan,
		mdiMusicNotePlus
	} from '@mdi/js';
	import { createEventDispatcher, onMount } from 'svelte';
	import { Button, Dialog, Field, Input, Progress, ProgressCircle } from 'svelte-ux';

	const dispatch = createEventDispatcher();

	export let video: Video;
	export let downloadPercent = 0;
	let thumbnailUrl = '';
	let videoUrl = '';
	let mp3Url = '';

	let innerWidth: number, innerHeight: number;

	let videoWidth = 480;
	let videoHeight = 270;

	let deleteButtonLoading = false;
	async function deleteVideoAsync() {
		deleteButtonLoading = true;
		let videosApi = new VideosApi();
		try {
			await videosApi.apiVideosDeleteVideoDelete({ videoId: video.id ?? -1 });
			dispatch('videoDeleted');
		} catch (error) {
			alert('Failed to delete video.');
			console.log('Failed to delete video: ', error);
		} finally {
			deleteButtonLoading = false;
		}
	}

	let editingName = false;
	let newName = '';
	let editButtonLoading = false;
	async function updateVideoNameAsync() {
		if (newName == '' || newName == video.fileName) {
			console.error('New name is empty.');
			return;
		}

		editButtonLoading = true;
		let videosApi = new VideosApi();
		try {
			await videosApi.apiVideosUpdateVideoNamePatch({
				updateVideoNameRequest: { videoId: video.id, newName }
			});
			video.fileName = newName;
			dispatch('videoModified');
			clearNameEditInputs();
		} catch (error) {
			alert('Failed to rename video');
			console.log('Failed to rename video: ', error);
		} finally {
			editButtonLoading = false;
		}
	}

	function clearNameEditInputs() {
		editingName = false;
		newName = video.fileName ?? '';
		editButtonLoading = false;
	}

	onMount(async () => {
		if (video.thumbnailName !== '') {
			thumbnailUrl = BASE_PATH + '/Downloads/' + video.thumbnailName;
		}
		videoUrl = BASE_PATH + '/Downloads/' + video.fileName;
		mp3Url =
			video.mp3FileName === ''
				? BASE_PATH + '/api/videos/GetMp3?videoId=' + video.id
				: BASE_PATH + '/Downloads/' + video.mp3FileName;

		videoWidth = 480 > innerWidth ? innerWidth : 480;
		videoHeight = videoWidth * (9 / 16);

		newName = video.fileName ?? '';
	});

	async function downloadResource(url: string, name = '') {
		try {
			const response = await fetch(url);
			const blob = await response.blob();

			const link = document.createElement('a');
			link.download = name;
			link.href = URL.createObjectURL(blob);

			link.click();

			URL.revokeObjectURL(link.href);
		} catch (error) {
			alert('Error downloading resource');
			console.error('Error downloading resource:', error);
		}
	}

	let generateMp3Loading = false;
	let downloadVideoLoading = false;
	let downloadMp3Loading = false;

	async function generateMp3(videoId: number) {
		let videosApi = new VideosApi();
		generateMp3Loading = true;
		await videosApi.apiVideosGenerateMp3Get({ videoId });
	}

	let dialogDeleteOpen = false;
</script>

<svelte:window bind:innerWidth bind:innerHeight />

<div
	style="width: {videoWidth}px;"
	class="flex flex-col shadow-md dark:shadow-slate-100/20 dark:shadow-sm rounded-lg overflow-hidden m-2"
>
	{#if !video.downloaded}
		<div
			style="width: {videoWidth}px; height: {videoHeight}px;"
			class="flex flex-col pt-10 justify-center items-center"
		>
			<ProgressCircle size={64} class="text-black dark:text-white" />
			<h2 class="overflow-ellipsis overflow-hidden whitespace-nowrap text-xl font-bold mb-2">
				Downloading: {downloadPercent}%
			</h2>
		</div>
		<div class="w-full">
			<Progress value={downloadPercent} max={100} />
		</div>
	{:else}
		<div style="width: {videoWidth}px; height: {videoHeight}px;">
			<video
				poster={thumbnailUrl}
				class="block max-w-full max-h-full w-full h-full"
				controls
				width={videoWidth}
				height={videoHeight}
			>
				<source src={videoUrl} type="video/mp4" />
				<track kind="captions" />
			</video>
		</div>
	{/if}

	<div class="flex flex-col justify-between p-4 w-full">
		<div>
			<div class="flex items-center justify-between">
				{#if editingName}
					<div class="flex w-full items-end gap-2">
						<Field class="w-full" label="New name" labelPlacement="top">
							<Input bind:value={newName} placeholder="Cool_video_1.mp4" />
						</Field>

						<Button
							loading={editButtonLoading}
							on:click={async () => await updateVideoNameAsync()}
							variant="fill"
							color="success"
							rounded
							icon={mdiCheck}
						></Button>
						<Button
							loading={editButtonLoading}
							on:click={() => clearNameEditInputs()}
							variant="fill"
							color="danger"
							rounded
							icon={mdiCancel}
						></Button>
					</div>
				{:else}
					<h2 class="overflow-ellipsis overflow-hidden whitespace-nowrap text-xl font-bold mb-2">
						{video.fileName}
					</h2>
					<Button
						on:click={() => (editingName = true)}
						disabled={!video.downloaded}
						variant="default"
						icon={mdiPencil}
					></Button>
				{/if}
			</div>

			<p class="text-sm">Downloaded: {formatDate(video.createdAt?.toString() ?? '')}</p>
			<p class="text-sm">
				Size: {video.downloaded ? formatBytes(video.size ?? 0) : 'Awaiting completion'}
			</p>
		</div>
		<div class="flex mt-4 gap-1 justify-center md:justify-normal">
			{#if video.mp3Status == Mp3Status.NONE}
				<Button
					disabled={!video.downloaded}
					variant={'fill'}
					color="accent"
					icon={mdiMusicNotePlus}
					class="text-slate-100"
					loading={generateMp3Loading}
					on:click={async () => await generateMp3(video.id ?? -1)}>Generate</Button
				>
			{:else}
				<Button
					disabled={!video.downloaded || video.mp3Status == Mp3Status.INPROGRESS}
					loading={video.mp3Status == Mp3Status.INPROGRESS || downloadMp3Loading}
					variant={'fill'}
					color="accent"
					icon={mdiDownload}
					class="text-slate-100"
					on:click={async () => {
						downloadVideoLoading = true;
						await downloadResource(
							BASE_PATH + `/Downloads/${video.mp3FileName}`,
							video.mp3FileName ?? ''
						);
						downloadVideoLoading = false;
					}}>Mp3</Button
				>
			{/if}
			<Button
				disabled={!video.downloaded}
				variant={'fill'}
				color="success"
				icon={mdiDownload}
				class="text-slate-100"
				loading={downloadVideoLoading}
				on:click={async () => {
					downloadVideoLoading = true;
					await downloadResource(BASE_PATH + `/Downloads/${video.fileName}`, video.fileName ?? '');
					downloadVideoLoading = false;
				}}
				>Download
			</Button>
			<Button
				disabled={!video.downloaded}
				variant={'fill'}
				color="danger"
				icon={mdiDelete}
				class="text-slate-100"
				on:click={() => (dialogDeleteOpen = true)}
				loading={deleteButtonLoading}>Delete</Button
			>
		</div>
	</div>

	<Dialog on:close={() => (dialogDeleteOpen = false)} bind:open={dialogDeleteOpen}>
		<div slot="title">Are you sure?</div>
		<div class="px-6 py-3">This will permanently delete the item and can not be undone.</div>
		<div class="gap-3 p-2" slot="actions">
			<Button
				class="mr-1"
				icon={mdiTrashCan}
				on:click={async () => await deleteVideoAsync()}
				variant="fill"
				color="success">Yes</Button
			>
			<Button class="mr-1" icon={mdiCancel} variant="fill" color="danger">Cancel</Button>
		</div>
	</Dialog>
</div>
