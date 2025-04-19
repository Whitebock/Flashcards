import { error } from '@sveltejs/kit';
import { API_URL, type Deck } from '$lib/types';
import type { PageLoad } from './$types';

export const load: PageLoad = async ({ fetch }) => {
	let decks: Array<Deck>;
	try {
		const response = await fetch(API_URL + "/decks");
		decks = await response.json();
	} 
	catch {
		error(504, "Api server is not reachable");
	}

	return { decks };
};