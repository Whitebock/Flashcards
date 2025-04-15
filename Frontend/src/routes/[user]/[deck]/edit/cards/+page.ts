import { API_URL, type Card } from '$lib/types';
import type { PageLoad } from './$types';

export const load: PageLoad = async ({ fetch, params, parent }) => {
    const deck = (await parent()).deck;
    const res = await fetch(`${API_URL}/decks/${deck.id}/cards`);
    const cards: Array<Card> = await res.json();

    return { cards };
};