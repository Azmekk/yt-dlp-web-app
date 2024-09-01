<script lang="ts">
	import {
		mdiArrowRightThin,
		mdiChevronDoubleLeft,
		mdiChevronDoubleRight,
		mdiChevronLeft,
		mdiChevronRight
	} from '@mdi/js';
	import { createEventDispatcher } from 'svelte';
	import { Button, Menu, TextField } from 'svelte-ux';

	const dispatch = createEventDispatcher();

	export let perPage: number = 0;
	export let total: number = 0;
	export let currentPage: number;

	let selectablePagesAmount = 5;

	$: totalPages = Math.ceil(total / perPage);

	let pageInputIsOpen = false;
	let pageInputValue = '';

	function getPageNumber(offset: number, currentPage: number): number {
		let minPage = Math.max(1, currentPage - 2);

		if (currentPage >= totalPages - 1) {
			minPage = Math.max(1, currentPage - 4 + (totalPages - currentPage));
		}

		const clampedOffset = Math.max(0, Math.min(offset, selectablePagesAmount - 1));

		return minPage + clampedOffset;
	}

	function updatePage(page: number) {
		if (currentPage == page) {
			return;
		}

		dispatch('pageUpdated');
		currentPage = page;
	}
</script>

<div class="flex h-10">
	<div class=" flex rounded-md shadow-md dark:shadow-slate-100/20 dark:shadow-sm">
		{#if totalPages > 5 && currentPage > 3}
			<Button on:click={() => updatePage(1)} class="mx-1" icon={mdiChevronDoubleLeft} />
		{/if}

		{#each { length: totalPages > selectablePagesAmount ? selectablePagesAmount : totalPages } as _, i}
			<button
				on:click={() => {
					updatePage(getPageNumber(i, currentPage));
				}}
				class="h-full mx-0.5 w-10 rounded-md hover:bg-primary-200 active:bg-primary-100 hover:cursor-pointer {getPageNumber(
					i,
					currentPage
				) == currentPage
					? 'bg-primary-200'
					: ''}"
			>
				{getPageNumber(i, currentPage)}
			</button>
		{/each}
		{#if totalPages > selectablePagesAmount && currentPage < totalPages - 2}
			<Button
				on:click={() => {
					pageInputIsOpen = !pageInputIsOpen;
					pageInputValue = currentPage.toString();
				}}>{pageInputIsOpen ? 'x' : '...'}</Button
			>
			<Menu
				placement="top"
				on:close={() => (pageInputIsOpen = false)}
				bind:open={pageInputIsOpen}
				explicitClose={true}
			>
				<div class="flex">
					<TextField type="integer" bind:value={pageInputValue} class="w-20" label="Page"
					></TextField>
					<Button
						on:click={() => {
							updatePage(Number(pageInputValue) > totalPages ? totalPages : Number(pageInputValue));
							pageInputIsOpen = false;
						}}
						rounded
						class="m3"
						icon={mdiArrowRightThin}
					></Button>
				</div>
			</Menu>
		{/if}

		{#if totalPages > 5 && currentPage < totalPages - 2}
			<Button on:click={() => updatePage(totalPages)} class="mx-1" icon={mdiChevronDoubleRight} />
		{/if}
	</div>
</div>
