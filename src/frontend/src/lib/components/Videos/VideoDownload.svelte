<script lang="ts">

	import { getFormattedVideoName } from '$lib/utils';
	import { mdiCancel, mdiDownload, mdiPlus } from '@mdi/js';
	import { createEventDispatcher } from 'svelte';
	import {
		Button,
		Dialog,
		Field,
		Input,
		SelectField,
		Switch,
		type MenuOption
	} from 'svelte-ux';

	const dispatch = createEventDispatcher();

	let downloadVideoDialogOpen = false;
	let formatOptions: MenuOption[] = [
		{
			value: 'mp4',
			label: 'mp4'
		}
	];

	let videoDimensions: VideoDimensionsResponse = {
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
		},
		
	];
	let selectedDimension = 1080;
	let dimensionsToggle = false;
	let maxDimension = 2160;

	let dialogLoading = false;
	let selectedFormat: string = 'mp4';
	let videoUrl: string = '';
	let fileName: string = '';

	function clearDialogFieldsAndClose() {
		selectedDimension = 1080;
		dimensionsToggle = false;
		videoDimensions = {
			width: 0,
			height: 0
		};
		selectedFormat = 'mp4';
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

		try {
			var nameResponse = await getYoutubeNameAsync(videoUrl);
			fileName = (nameResponse == null || nameResponse?.title == "") ? getFormattedVideoName() : nameResponse?.title;
		} catch (error) {
			console.error('Failed to fetch video name.');
		}
	}

	async function fetchDimensionsAsync() {
		if (!dimensionsToggle || videoUrl == "") {
			return;
		}

		try {
			var dimensionsResponse = await getVideoDimensionsAsync(videoUrl);
			maxDimension = dimensionsResponse.height ?? 2160;
		} catch (error) {
			console.error('Failed to fetch video max dimensions.');
		}
	}

	async function getVideoInfoAsync() {
		dialogLoading = true;
		await fetchNameAsync();
		await fetchDimensionsAsync();
		dialogLoading = false;
	}

	let saveVideoButtonLoading = false;
	async function submitVideoSaveAsync() {
		saveVideoButtonLoading = true;
		try {
			if (dimensionsToggle) {
				await saveVideoAsync(videoUrl, fileName, selectedFormat, selectedDimension);
			}
			else{
				await saveVideoAsync(videoUrl, fileName, selectedFormat);
			}
			
		} catch (error) {
			dispatch('videoSaveFail', {error: error});
			console.error(error);
		} finally {
			dispatch('videoSaved');
			clearDialogFieldsAndClose();
		}
	}
</script>

<div class="fixed bottom-10 md:bottom-20 right-2 md:right-10 p-4 z-10">
	<Button class="text-slate-100" size="lg" on:click={() => (downloadVideoDialogOpen = true)} icon={mdiPlus} rounded="full" variant="fill" color="primary" > New video</Button>
</div>

<div class="w-full absolute">
	<Dialog
		loading={dialogLoading || saveVideoButtonLoading}
		class="p-5 w-full sm:w-1/2 xl:w-1/3"
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

			<div class="flex w-full mb-5">
				<Field class="w-full mr-2" label="File name" labelPlacement="top">
					<Input bind:value={fileName} required={true} placeholder="video_name_example" />
				</Field>
				<div>
					<SelectField
						required={true}
						placeholder="eg. mp4"
						bind:value={selectedFormat}
						options={formatOptions}
						label="Media format"
						labelPlacement="top"
						clearable={false}
					></SelectField>
				</div>
			</div>

			{#if (selectedFormat == 'mp4')}
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
			{/if}

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
