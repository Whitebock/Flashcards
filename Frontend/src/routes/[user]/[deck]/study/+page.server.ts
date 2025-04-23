import { API_URL, type Card } from '$lib/types';
import type { PageServerLoad, Actions } from './$types';

export const load: PageServerLoad = async ({ fetch, params, parent }) => {
    const deck = (await parent()).deck;
    const res = await fetch(`${API_URL}/decks/${deck.id}/study`);
    const result: {
        card: Card,
        left: number
    } = await res.json();

    return result;
};

export const actions = {
	default: async ({request}) => {
        let data = await request.formData();

        if (data.has("answer")) {
            return {
                showAnswer: data.get("answer") == "show"
            }
        }
        else if(data.has("choice")) {
            const cardId = data.get("cardId");
            await fetch(`${API_URL}/cards/${cardId}`, {
                method: 'PATCH',
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    choice: data.get("choice")
                })
            })
        }
	}
} satisfies Actions;