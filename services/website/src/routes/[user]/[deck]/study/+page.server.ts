import { error } from '@sveltejs/kit';
import type { PageServerLoad, Actions } from './$types';
import type { NullableOfCardStatus } from '$lib/api/schema';

export const load: PageServerLoad = async ({ parent, locals }) => {
    const deck = (await parent()).deck;
    const { data } = await locals.api.GET('/decks/{deckId}/study', {params: {
        path: {
            deckId: deck.id!
        }
    }});

    if(!data) error(500);

    return data;
};

export const actions = {
	default: async ({ request, locals }) => {
        let data = await request.formData();

        if (data.has("answer")) {
            return {
                showAnswer: data.get("answer") == "show"
            }
        }
        else if(data.has("choice")) {
            const cardId = data.get("cardId")?.toString() ?? error(400);
            const choice = data.get("choice")?.toString() as NullableOfCardStatus|undefined ?? error(400);

            await locals.api.PATCH('/cards/{cardId}', {
                params: {path: {cardId: cardId}},
                body: {
                    status: choice
                }
            })
        }
	}
} satisfies Actions;