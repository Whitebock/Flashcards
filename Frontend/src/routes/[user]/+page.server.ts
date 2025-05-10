import { error } from '@sveltejs/kit';
import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async ({ params, locals }) => {
    const { error: apiError, data: profile } = await locals.api.GET(`/users/{username}`, {params: {path: {
        username: params.user
    }}});

    if(apiError) {
        console.error(apiError);
        error(500, "Unable to fetch user");
    }

    const { data: decks } = await locals.api.GET('/decks', {params: {
        query: {
            user: profile.username
        }
    }})

    return { profile, decks };
};