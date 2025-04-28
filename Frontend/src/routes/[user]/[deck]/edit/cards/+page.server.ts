import api from '$lib/api';
import { error } from '@sveltejs/kit';
import type { Actions, PageServerLoad } from './$types';

export const load: PageServerLoad = async ({ parent, locals }) => {
    const { deck } = await parent();
    const { data: cards } = await locals.api.GET('/decks/{deckId}/cards', {params: {path: {
        deckId: deck.id!
    }}})

    return { cards: cards ?? [] };
}

export const actions = {
    update: async ({ request, locals }) => { 
        let data = await request.formData();
        const cardId = data.get("cardId")?.toString() ?? error(400);
        const front = data.get("front")?.toString() ?? error(400);
        const back = data.get("back")?.toString() ?? error(400);

        await locals.api.PUT('/cards/{cardId}', {
            params: {path: {
                cardId
            }},
            body: {
                front,
                back
            }
        });
    },
    remove: async ({ request, locals }) => { 
        let data = await request.formData();
        const cardId = data.get("cardId")?.toString() ?? error(400);

        await locals.api.DELETE('/cards/{cardId}', {params: {path: {
            cardId
        }}});
    },
    add: async ({ request, locals }) => {
        let data = await request.formData();
        const deckId = data.get("deckId")?.toString() ?? error(400);
        const front = data.get("front")?.toString() ?? error(400);
        const back = data.get("back")?.toString() ?? error(400);
        
        await locals.api.POST('/cards', { 
            body: {
                deckId: deckId,
                front,
                back
            }
        });
    }
} satisfies Actions;