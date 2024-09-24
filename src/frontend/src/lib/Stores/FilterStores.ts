import { VideoOrderBy } from '$lib/index';
import { writable } from 'svelte/store';

export const orderByStore = writable<VideoOrderBy>(VideoOrderBy.CreationDate);
export const orderByDescendingStore = writable(true);
export const videoSearchStore = writable("");

