import { API_URL, type Card, type Deck } from '$lib/types';
import type { LayoutLoad } from './$types';

export const load: LayoutLoad = async ({ fetch, params }) => {
    const res = await fetch( `${API_URL}/decks/search?user=${params.user}&name=${params.deck}`);
    const deck: Deck = await res.json();

    return { deck };
};