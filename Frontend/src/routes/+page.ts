import { API_URL, type Deck } from '$lib/types';
import type { PageLoad } from './$types';

export const load: PageLoad = async ({ fetch }) => {
	const res = await fetch(API_URL + "/decks");
	const decks: Array<Deck> = await res.json();

	return { decks };
};