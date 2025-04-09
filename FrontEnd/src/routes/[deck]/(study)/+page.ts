import { API_URL, type Card } from '$lib/types';
import type { PageLoad } from './$types';

export const load: PageLoad = async ({ fetch, params }) => {
    const res = await fetch(`${API_URL}/decks/${params.deck}/study`);
    const result: {
        card: Card,
        left: number
    } = await res.json();

    return result;
};