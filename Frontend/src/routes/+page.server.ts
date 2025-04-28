import { error } from '@sveltejs/kit';
import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async ({ locals }) => {
	try {
		let { data: reccomended } = await locals.api.GET('/decks/recommended');
		return reccomended;
	} 
	catch {
		error(504, "Api server is not reachable");
	}
};