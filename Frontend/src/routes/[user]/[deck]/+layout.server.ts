import type { LayoutServerLoad } from './$types';
import { error } from '@sveltejs/kit';

export const load: LayoutServerLoad = async ({ params, locals }) => {
    const { data: decks } = await locals.api.GET('/decks/search', {params: {query: {
        username: params.user,
        deckname: params.deck
    }}});

    if(!decks || decks.length == 0) {
        error(404, "Deck was not found");
    }

    return { deck: decks[0]! };
};