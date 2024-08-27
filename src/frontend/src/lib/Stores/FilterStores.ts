import { writable } from 'svelte/store';
import { VideoOrderByParam } from '$lib/api_client'

export const orderByStore = writable<VideoOrderByParam>(VideoOrderByParam.Date);
export const orderByDescendingStore = writable(true);
export const videoSearchStore = writable("");

