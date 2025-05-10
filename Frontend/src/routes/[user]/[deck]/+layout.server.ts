import type { LayoutServerLoad } from './$types';
import { error } from '@sveltejs/kit';

export const load: LayoutServerLoad = async ({ params, locals }) => {
    const { data: decks } = await locals.api.GET('/decks', {params: {query: {
        user: params.user,
        name: params.deck,
        amount: 1
    }}});

    if(!decks || decks.length == 0) {
        error(404, "Deck was not found");
    }

    return { deck: decks[0]! };
};