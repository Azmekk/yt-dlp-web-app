<script lang="ts">
	import { formatBytes, formatDate } from '$lib/utils';
	import {
		mdiCancel,
		mdiCheck,
		mdiDelete,
		mdiDownload,
		mdiPencil,
		mdiTrashCan
	} from '@mdi/js';
	import { createEventDispatcher, onMount } from 'svelte';
	import { Button, Dialog, Field, Input, Progress, ProgressCircle } from 'svelte-ux';

	const dispatch = createEventDispatcher();

	export let video: ApiVideoResponse;
	let thumbnailUrl = '';
	let videoUrl = '';

	let innerWidth: number, innerHeight: number;

	let videoWidth = 480;
	let videoHeight = 270;

	let deleteButtonLoading = false;
	async function deleteVideoAsync() {
		deleteButtonLoading = true;
		try {
			await client_deleteVideo(video.id);
			dispatch('videoDeleted');
		} catch (error) {
			console.log('Failed to delete video: ', error);
		} finally {
			deleteButtonLoading = false;
		}
	}

	let editingName = false;
	let newName = '';
	let editButtonLoading = false;
	async function updateVideoNameAsync() {
		if (newName == '' || newName == video.name) {
			console.error('New name is empty.');
			return;
		}

		editButtonLoading = true;

		try {
			await renameVideoAsync(video.id, newName);
            video.name = newName;
			dispatch('videoModified');
            clearNameEditInputs();
		} catch (error) {
			console.log('Failed to rename video: ', error);
		} finally {
			editButtonLoading = false;
		}
	}

    function clearNameEditInputs(){
        editingName = false;
        newName = video.name;
        editButtonLoading = false;
    }

	onMount(() => {
		if (video.thumbnailName !== '') {
			thumbnailUrl = getThumbnailPath(video.id);
		}
		videoUrl = getVideoPath(video.id);

		videoWidth = 480 > innerWidth ? innerWidth : 480;
		videoHeight = videoWidth * (9 / 16);

		newName = video.name;
	});

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
				Downloading: {video.downloadPercent}%
			</h2>
		</div>
		<div class="w-full">
			<Progress value={video.downloadPercent} max={100} />
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
						{video.name}
					</h2>
					<Button
						on:click={() => (editingName = true)}
						disabled={!video.downloaded}
						variant="default"
						icon={mdiPencil}
					></Button>
				{/if}
			</div>

			<p class="text-sm">Downloaded: {formatDate(video.creation_time)}</p>
			<p class="text-sm">
				Size: {video.downloaded ? formatBytes(video.size) : 'Awaiting completion'}
			</p>
		</div>
		<div class="flex mt-4 gap-1 justify-center md:justify-normal">
			<a download href={getMp3DownloadPath(video.id)}>
				<Button
					disabled={!video.downloaded}
					variant={'fill'}
					color="accent"
					icon={mdiDownload}
					class="text-slate-100"
					rounded>Mp3</Button
				>
			</a>
			<a download href={getDownloadVideoPath(video.id)}>
				<Button
					disabled={!video.downloaded}
					variant={'fill'}
					color="success"
					icon={mdiDownload}
					class="text-slate-100">Download</Button
				>
			</a>
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
