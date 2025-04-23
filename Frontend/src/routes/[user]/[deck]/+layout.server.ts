import { API_URL, type Deck } from '$lib/types';
import type { LayoutServerLoad } from './$types';

export const load: LayoutServerLoad = async ({ fetch, params }) => {
    const res = await fetch( `${API_URL}/decks/search?user=${params.user}&name=${params.deck}`);
    const deck: Deck = await res.json();

    return { deck };
};