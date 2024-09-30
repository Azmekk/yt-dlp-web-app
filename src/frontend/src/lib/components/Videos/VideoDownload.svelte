<script lang="ts">
	import { VideosApi } from '$lib/api-clients/backend-client';
	import type { SaveVideoRequest, VideoDimensions, VideoDuration } from '$lib/api-clients/backend-client/models';

	import {
		getFormattedVideoName,
		getResolutionDimensions,
		secondsToTimeString,
		timeStringToSeconds
	} from '$lib/utils';
	import { mdiCancel, mdiDownload, mdiPlus, mdiAutorenew } from '@mdi/js';
	import { createEventDispatcher } from 'svelte';
	import { Button, Dialog, Field, Input, SelectField, Switch, RangeSlider, type MenuOption } from 'svelte-ux';

	const dispatch = createEventDispatcher();

	let downloadVideoDialogOpen = false;
	let formatOptions: MenuOption[] = [
		{
			value: 'mp4',
			label: 'mp4'
		}
	];

	let videoDimensions: VideoDimensions = {
		width: 0,
		height: 0
	};

	let resolutionOptions: MenuOption[] = [
		{
			value: 2160,
			label: '4K (3840x2160)'
		},
		{
			value: 1440,
			label: '1440p (2560x1440)'
		},
		{
			value: 1080,
			label: '1080p (1920x1080)'
		},
		{
			value: 720,
			label: '720p (1280x720)'
		},
		{
			value: 480,
			label: '480p (854x480)'
		},
		{
			value: 360,
			label: '360p (640x360)'
		},
		{
			value: 240,
			label: '240p (426x240)'
		},
		{
			value: 144,
			label: '144p (256x144)'
		}
	];
	let selectedDimension = 1080;
	let dimensionsToggle = false;
	let cutVideoToggle = false;
	let maxDimension = 2160;

	let videoDurationInSeconds = 0;

	let dialogLoading = false;
	//let selectedFormat: string = 'mp4';
	let videoUrl: string = '';
	let fileName: string = '';

	let videoDuration: VideoDuration = {
		startTime: '00:00:00',
		endTime: '00:00:00'
	};

	function clearDialogFieldsAndClose() {
		selectedDimension = 1080;
		dimensionsToggle = false;
		videoDimensions = {
			width: 0,
			height: 0
		};
		//selectedFormat = 'mp4';
		videoUrl = '';
		fileName = '';
		downloadVideoDialogOpen = false;
		saveVideoButtonLoading = false;
		cutVideoToggle = false;
		maxDimension = 2160;

		videoDurationInSeconds = 0;
	}

	let lastFetchedUrl = '';
	async function fetchVideoData() {
		if (videoUrl == '') {
			console.error("Can't get video name, video url is empty.");
			return;
		}

		if (videoUrl == lastFetchedUrl) {
			return;
		}

		let videoApi = new VideosApi();

		try {
			var videoDataResponse = await videoApi.apiVideosGetVideoDataGet({ videoUrl: videoUrl });
			fileName =
				videoDataResponse == null ||
				videoDataResponse.title == null ||
				videoDataResponse.title == ''
					? getFormattedVideoName()
					: videoDataResponse?.title.substring(0, 80);

			videoDurationInSeconds = videoDataResponse?.duration ?? 0;
			videoDuration.startTime = '00:00:00';
			videoDuration.endTime = secondsToTimeString(videoDataResponse?.duration ?? 0);

			lastFetchedUrl = videoUrl;
		} catch (error) {
			alert('Failed to fetch video data.');
			console.error(error);
		}
	}

	async function fetchDimensionsAsync() {
		if (!dimensionsToggle || videoUrl == '') {
			return;
		}

		let videoApi = new VideosApi();

		try {
			var dimensionsResponse = await videoApi.apiVideosGetMaxDimensionsGet({ videoUrl: videoUrl });
			maxDimension = dimensionsResponse.height ?? 2160;
		} catch (error) {
			alert('Failed to fetch video max dimensions.');
			console.error(error);
		}
	}

	async function getVideoInfoAsync() {
		dialogLoading = true;
		if (dimensionsToggle) {
			await fetchDimensionsAsync();
		}
		if (cutVideoToggle) {
			await fetchVideoData();
		}
		dialogLoading = false;
	}

	let saveVideoButtonLoading = false;
	async function submitVideoSaveAsync() {
		saveVideoButtonLoading = true;
		let videoApi = new VideosApi();
		try {
			let saveVideoRequest: SaveVideoRequest = {
				videoUrl: videoUrl,
				videoName: fileName
			}

			if (dimensionsToggle) {
				saveVideoRequest.videoDimensions = getResolutionDimensions(selectedDimension) ?? undefined;
			}

			if (cutVideoToggle) {
				saveVideoRequest.videoDuration = videoDuration;
			}

			videoApi.apiVideosSaveVideoPost({saveVideoRequest});
		} catch (error) {
			alert('Video save failed.');
			dispatch('videoSaveFail', { error: error });
			console.error(error);
		} finally {
			dispatch('videoSaved');
			clearDialogFieldsAndClose();
		}
	}

	function validateTimeInput(event: any) {
		const input = event.target;
		const value = input?.value;

		const validCharacters = /^[0-9:.]*$/;
		if (!validCharacters.test(value)) {
			input.value = value.slice(0, -1);
			return;
		}

		const timeFormat = /^(\d{2}:\d{2}:\d{2}|\d{2}:\d{2}:\d{2}\.\d{3})$/;
		if (!timeFormat.test(value) && value.length > 0) {
			alert('Invalid format. Use 00:00:00 or 00:00:00.000');
		}
	}

	let generateNameButtonLoading = false;
</script>

<div class="fixed bottom-10 md:bottom-15 right-2 md:right-10 p-4 z-10">
	<Button
		class="text-slate-100"
		size="lg"
		on:click={() => (downloadVideoDialogOpen = true)}
		icon={mdiPlus}
		rounded="full"
		variant="fill"
		color="primary">New Video</Button
	>
</div>

<div class="w-full absolute">
	<Dialog
		loading={dialogLoading || saveVideoButtonLoading}
		class="p-5 w-full sm:w-3/4 2xl:w-1/3"
		bind:open={downloadVideoDialogOpen}
		on:close={clearDialogFieldsAndClose}
	>
		<form on:submit={async () => submitVideoSaveAsync()}>
			<div class="flex w-full mb-5">
				<Field class="w-full" label="Video url" labelPlacement="top">
					<Input
						bind:value={videoUrl}
						required={true}
						placeholder="eg. https://www.youtube.com/watch?v=dQw4w9WgXcQ"
						on:input={async () => await getVideoInfoAsync()}
					/>
				</Field>
			</div>

			<div class="flex w-full mb-2">
				<Field class="w-full" label="File name" labelPlacement="top">
					<Input bind:value={fileName} required={true} placeholder="video_name_example" />
				</Field>
			</div>

			<div class="mb-4 w-full">
				<Button
					variant="fill"
					color="accent"
					icon={mdiAutorenew}
					class="text-slate-100 mr-1 w-full"
					loading={generateNameButtonLoading}
					on:click={async () => {
						generateNameButtonLoading = true;
						await fetchVideoData();
						generateNameButtonLoading = false;
					}}>Generate Name</Button
				>
			</div>

			<div class="w-full mb-2">
				<div class="mb-1">
					<div class="pl-0.5 text-sm">Select dimensions</div>
					<Switch
						on:change={async () => {
							dialogLoading = true;
							await fetchDimensionsAsync();
							dialogLoading = false;
						}}
						bind:checked={dimensionsToggle}
					/>
				</div>
				{#if dimensionsToggle}
					<div>
						<SelectField
							required={dimensionsToggle}
							placeholder="eg. 1080p"
							bind:value={selectedDimension}
							options={resolutionOptions.filter((x) => x.value <= maxDimension)}
							label="Media dimensions"
							labelPlacement="top"
						></SelectField>
					</div>
				{/if}
			</div>

			<div class="w-full mb-2">
				<div class="mb-1">
					<div class="pl-0.5 text-sm">Cut video</div>
					<Switch
						on:change={async () => {
							dialogLoading = true;
							await fetchVideoData();
							dialogLoading = false;
						}}
						bind:checked={cutVideoToggle}
					/>
				</div>
				{#if cutVideoToggle}
					<div class="flex w-full mb-1">
						<Field class="w-1/2 pl-2" label="Start time" labelPlacement="top">
							<input
								class="h-full bg-transparent focus:outline-none focus:ring-0"
								type="text"
								bind:value={videoDuration.startTime}
								on:change={(e) => {
									validateTimeInput(e);
									if (timeStringToSeconds(videoDuration.startTime ?? '00:00:00') < 0) {
										videoDuration.startTime = '00:00:00';
									}
								}}
							/>
						</Field>
						<Field class="w-1/2 pl-2" label="End time" labelPlacement="top">
							<input
								class="h-full bg-transparent focus:outline-none focus:ring-0"
								type="text"
								bind:value={videoDuration.endTime}
								on:change={(e) => {
									validateTimeInput(e);
									const inputDurationInSeconds = timeStringToSeconds(
										videoDuration.endTime ?? '00:00:00'
									);
									if (
										inputDurationInSeconds > videoDurationInSeconds ||
										inputDurationInSeconds <
											timeStringToSeconds(videoDuration.startTime ?? '00:00:00')
									) {
										videoDuration.endTime = secondsToTimeString(videoDurationInSeconds);
									}
								}}
							/>
						</Field>
					</div>
				{/if}
			</div>

			<div class="flex justify-center sm:justify-start">
				<Button
					type="submit"
					variant="fill"
					color="success"
					icon={mdiDownload}
					class="text-slate-100 mr-1"
					loading={saveVideoButtonLoading}>Download</Button
				>
				<Button
					on:click={() => clearDialogFieldsAndClose()}
					variant="fill"
					color="danger"
					icon={mdiCancel}
					class="text-slate-100">Cancel</Button
				>
			</div>
		</form>
	</Dialog>
</div>
