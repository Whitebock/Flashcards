import { error } from '@sveltejs/kit';
import { API_URL, type Deck } from '$lib/types';
import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async ({ fetch }) => {
	try {
		const response = await fetch(API_URL + "/decks/reccomended");
		let data: {
			popular: Array<Deck>,
			recent: Array<Deck>
		} = await response.json();
		return data;
	} 
	catch {
		error(504, "Api server is not reachable");
	}
};