<script lang="ts">
	import { VideosApi } from '$lib/api-clients/backend-client';
	import type { VideoDimensions } from '$lib/api-clients/backend-client/models';

	import { getFormattedVideoName, getResolutionDimensions } from '$lib/utils';
	import { mdiCancel, mdiDownload, mdiPlus, mdiAutorenew } from '@mdi/js';
	import { createEventDispatcher } from 'svelte';
	import { Button, Dialog, Field, Input, SelectField, Switch, type MenuOption } from 'svelte-ux';

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
	let maxDimension = 2160;

	let dialogLoading = false;
	//let selectedFormat: string = 'mp4';
	let videoUrl: string = '';
	let fileName: string = '';

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
	}

	async function fetchNameAsync() {
		if (videoUrl == '') {
			console.error("Can't get video name, video url is empty.");
			return;
		}

		let videoApi = new VideosApi();

		try {
			var nameResponse = await videoApi.apiVideosGetNameGet({ videoUrl: videoUrl });
			fileName =
				nameResponse == null || nameResponse.name == null || nameResponse.name == ''
					? getFormattedVideoName()
					: nameResponse?.name.substring(0, 80);
		} catch (error) {
			alert('Failed to fetch video name.');
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
		dialogLoading = false;
	}

	let saveVideoButtonLoading = false;
	async function submitVideoSaveAsync() {
		saveVideoButtonLoading = true;
		let videoApi = new VideosApi();
		try {
			if (dimensionsToggle) {
				//videoUrl, fileName, selectedFormat, selectedDimension
				await videoApi.apiVideosSaveVideoPost({
					saveVideoRequest: {
						videoUrl,
						videoName: fileName,
						videoDimensions: getResolutionDimensions(selectedDimension) ?? undefined
					}
				});
			} else {
				await videoApi.apiVideosSaveVideoPost({
					saveVideoRequest: { videoUrl, videoName: fileName }
				});
			}
		} catch (error) {
			alert('Video save failed.');
			dispatch('videoSaveFail', { error: error });
			console.error(error);
		} finally {
			dispatch('videoSaved');
			clearDialogFieldsAndClose();
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
						await fetchNameAsync();
						generateNameButtonLoading = false;
					}}>Generate Name</Button
				>
			</div>

			<div class="w-full mb-5">
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
