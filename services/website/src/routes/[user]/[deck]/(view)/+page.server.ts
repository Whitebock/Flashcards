import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async ({ parent, locals }) => {
    const { deck } = await parent();
    const { data: cards } = await locals.api.GET('/decks/{deckId}/cards', {params: {path: {
        deckId: deck.id!
    }}})

    return { cards: cards ?? [] };
}