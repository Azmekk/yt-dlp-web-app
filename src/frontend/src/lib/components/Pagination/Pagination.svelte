<script lang="ts">
	import {
		mdiArrowRightThin,
		mdiChevronDoubleLeft,
		mdiChevronDoubleRight,
		mdiChevronLeft,
		mdiChevronRight
	} from '@mdi/js';
	import { Button, Menu, TextField } from 'svelte-ux';
	import { createEventDispatcher } from 'svelte';

	const dispatch = createEventDispatcher();

	export let perPage: number = 0;
	export let total: number = 0;
	export let currentPage: number;

	let selectablePagesAmount = 5;

	$: pages = Math.ceil(total / perPage);

	let pageInputIsOpen = false;
	let pageInputValue = '';

	function getPageNumber(offset: number, currentPage: number): number {
		const minPage = Math.max(1, currentPage - 2);
		const maxPage = Math.min(pages, currentPage + 2);
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
		{#if pages > 5 && currentPage > 3}
			<Button on:click={() => updatePage(1)} class="mx-1" icon={mdiChevronDoubleLeft} />
		{/if}
		<Button
			disabled={currentPage == 1}
			on:click={() => updatePage(currentPage - 1)}
			class="mx-1"
			icon={mdiChevronLeft}
		/>

		{#each { length: pages > selectablePagesAmount ? selectablePagesAmount : pages } as _, i}
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
		{#if pages > selectablePagesAmount}
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
					<TextField
						type="integer"
						bind:value={pageInputValue}
						class="w-20"
						label="Page"
					></TextField>
					<Button
						on:click={() => {
							updatePage(Number(pageInputValue));
							pageInputIsOpen = false;
						}}
						rounded
						class="m3"
						icon={mdiArrowRightThin}
					></Button>
				</div>
			</Menu>
		{/if}
		<Button
			disabled={currentPage >= pages}
			on:click={() => updatePage(currentPage + 1)}
			class="mx-1"
			icon={mdiChevronRight}
		/>
		{#if pages > 5 && currentPage < pages - 2}
			<Button on:click={() => updatePage(pages)} class="mx-1" icon={mdiChevronDoubleRight} />
		{/if}
	</div>
</div>
