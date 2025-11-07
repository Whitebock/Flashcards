import { error } from '@sveltejs/kit';
import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async ({ locals }) => {
	try {
		let { data: popular } = await locals.api.GET('/decks', {params: {query: {
			amount: 4,
			sort: "popular"
		}}});
		let { data: recent } = await locals.api.GET('/decks', {params: {query: {
			amount: 4,
			sort: "new"
		}}});
		let { data: feed } = await locals.api.GET('/feed');
		if(!popular || !recent || !feed) error(504, "Got an invalid response from the Api server");
		return { popular, recent, feed };
	} 
	catch {
		error(504, "Api server is not reachable");
	}
};