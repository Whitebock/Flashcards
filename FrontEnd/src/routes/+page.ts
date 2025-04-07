import type { Deck } from '$lib/types';
import type { PageLoad } from './$types';

export const load: PageLoad = async ({ fetch }) => {
    const url = "http://localhost:5124";
	const res = await fetch(url + "/decks");
	const decks: Array<Deck> = await res.json();

	return { decks };
};