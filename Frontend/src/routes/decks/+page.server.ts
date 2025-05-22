import { error } from '@sveltejs/kit';
import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async ({ locals }) => {
    try {
        let { data: decks } = await locals.api.GET('/decks', {params: {query: {
            amount: 100
        }}});
        if(!decks) error(504, "Got an invalid response from the Api server");
        return { decks };
    } 
    catch {
        error(504, "Api server is not reachable");
    }
};