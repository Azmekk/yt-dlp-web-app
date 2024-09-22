<script lang="ts">
	import { mdiHarddisk, mdiMagnify, mdiTuneVariant } from '@mdi/js';
	import {
		AppBar,
		AppLayout,
		Button,
		Dialog,
		Field,
		Icon,
		Input,
		NavItem,
		ProgressCircle,
		Radio,
		settings,
		ThemeSelect,
		Tooltip
	} from 'svelte-ux';

	import * as backendClient from '$lib/api-clients/backend-client/index'
	import { page } from '$app/stores';
	import { orderByDescendingStore, orderByStore, videoSearchStore } from '$lib/Stores/FilterStores';
	import { usedStorageStore } from '$lib/Stores/UsedStorageStore';
	import { formatBytes } from '$lib/utils';
	import { onMount } from 'svelte';
	import '../app.postcss';

	settings({
		components: {
			AppBar: {
				classes: 'bg-primary text-white shadow-md'
			},
			AppLayout: {
				classes: {
					nav: 'bg-neutral-800 py-2'
				}
			},
			NavItem: {
				classes: {
					root: 'text-sm text-gray-400 pl-6 py-2 hover:text-white hover:bg-gray-300/10 [&:where(.is-active)]:text-sky-400 [&:where(.is-active)]:bg-gray-500/10'
				}
			}
		}
	});

	let usedStorage = 0;
	usedStorageStore.subscribe((x) => {
		usedStorage = x;
	});

	let orderBy: VideoOrderByParam = VideoOrderByParam.Date;
	orderByStore.subscribe((x) => {
		orderBy = x;
	});

	let orderByDescending: boolean = true;
	orderByDescendingStore.subscribe((x) => {
		orderByDescending = x;
	});

	let videoSearch: string = '';
	videoSearchStore.subscribe((x) => {
		videoSearch = x;
	});

	let orderByDialogOpen = false;

	let usedStorageLoading = false;
	async function updateUsedStorageAsync() {
		usedStorageLoading = true;
		try {
			let response = await getUsedStorageAsync();
			usedStorageStore.update((x) => (x = response.usedStorage));
		} catch (error) {
			console.error('Something went wrong when fetching storage usage: ', error);
		} finally {
			usedStorageLoading = false;
		}
	}

	function onVideoSearchInput(e: Event) {
		var element = e.target as HTMLInputElement;
		videoSearchStore.update((x) => (x = element.value));
	}

	onMount(async () => {
		await updateUsedStorageAsync();
	});
</script>

<AppLayout navWidth={240}>
	<svelte:fragment slot="nav">
		<NavItem
			path="/"
			text="Home"
			icon="M10,20V14H14V20H19V12H22L12,3L2,12H5V20H10Z"
			currentUrl={$page.url}
		/>

		<!--<button
			on:click={(e) => e.preventDefault()}
			class="hover:cursor-not-allowed opacity-50 hover:bg-primary w-full"
		>
			<NavItem
				class="hover:cursor-not-allowed"
				text="Settings (Not implemented)"
				icon={mdiCog}
				currentUrl={$page.url}
				path="/"
			/>
		</button>-->

		<div class="w-full border-t mt-5 p-5 border-slate-500 flex gap-2 text-white">
			<Icon class="" data={mdiHarddisk} />
			<p>Used: {!usedStorageLoading ? formatBytes(usedStorage) : ''}</p>
			{#if usedStorageLoading}
				<ProgressCircle class="text-white" size={20} />
			{/if}
		</div>
	</svelte:fragment>

	<AppBar>
		<Field rounded={true} icon={mdiMagnify} class="w-full sm:w-1/2 xl:w-1/4 mr-2">
			<Input
				on:input={onVideoSearchInput}
				class="text-black dark:text-gray-100"
				placeholder="Video search"
			/>
		</Field>
		<Tooltip title="Filter" placement="right" offset={5}>
			<Button
				icon={mdiTuneVariant}
				on:click={() => {
					orderByDialogOpen = true;
				}}
			/>
		</Tooltip>
		<div slot="actions" class="flex gap-3">
			<ThemeSelect lightThemes={['light']} darkThemes={['dark']} />
			<Tooltip title="View repository" placement="left" offset={2}>
				<Button
					icon="M12,2A10,10 0 0,0 2,12C2,16.42 4.87,20.17 8.84,21.5C9.34,21.58 9.5,21.27 9.5,21C9.5,20.77 9.5,20.14 9.5,19.31C6.73,19.91 6.14,17.97 6.14,17.97C5.68,16.81 5.03,16.5 5.03,16.5C4.12,15.88 5.1,15.9 5.1,15.9C6.1,15.97 6.63,16.93 6.63,16.93C7.5,18.45 8.97,18 9.54,17.76C9.63,17.11 9.89,16.67 10.17,16.42C7.95,16.17 5.62,15.31 5.62,11.5C5.62,10.39 6,9.5 6.65,8.79C6.55,8.54 6.2,7.5 6.75,6.15C6.75,6.15 7.59,5.88 9.5,7.17C10.29,6.95 11.15,6.84 12,6.84C12.85,6.84 13.71,6.95 14.5,7.17C16.41,5.88 17.25,6.15 17.25,6.15C17.8,7.5 17.45,8.54 17.35,8.79C18,9.5 18.38,10.39 18.38,11.5C18.38,15.32 16.04,16.16 13.81,16.41C14.17,16.72 14.5,17.33 14.5,18.26C14.5,19.6 14.5,20.68 14.5,21C14.5,21.27 14.66,21.59 15.17,21.5C19.14,20.16 22,16.42 22,12A10,10 0 0,0 12,2Z"
					href="https://github.com/azmekk"
					class="p-2"
					target="_blank"
				/>
			</Tooltip>
		</div>
	</AppBar>

	<Dialog
		bind:open={orderByDialogOpen}
		on:close={() => {
			orderByDialogOpen = false;
		}}
	>
		<div class="text-center" slot="title">Ordering</div>
		<div class="p-5 flex gap-10">
			<div class="flex flex-col gap-4">
				<div>Order by</div>
				<div class="w-full border-b border-black"></div>
				<Radio
					name="label"
					size="lg"
					bind:group={orderBy}
					value={VideoOrderByParam.Date}
					on:change={() => orderByStore.update((x) => (x = orderBy))}>Date</Radio
				>
				<Radio
					name="label"
					size="lg"
					bind:group={orderBy}
					value={VideoOrderByParam.Size}
					on:change={() => orderByStore.update((x) => (x = orderBy))}>Size</Radio
				>
				<Radio
					name="label"
					size="lg"
					bind:group={orderBy}
					value={VideoOrderByParam.Title}
					on:change={() => orderByStore.update((x) => (x = orderBy))}>Title</Radio
				>
			</div>
			<div class="flex flex-col gap-4">
				<div>Direction</div>
				<div class="w-full border-b border-black"></div>
				<Radio
					name="label"
					size="lg"
					bind:group={orderByDescending}
					value={false}
					on:change={() => orderByDescendingStore.update((x) => (x = orderByDescending))}
					>Ascending</Radio
				>
				<Radio
					name="label"
					size="lg"
					bind:group={orderByDescending}
					value={true}
					on:change={() => orderByDescendingStore.update((x) => (x = orderByDescending))}
					>Descending</Radio
				>
			</div>
		</div>
	</Dialog>

	<slot />
</AppLayout>
