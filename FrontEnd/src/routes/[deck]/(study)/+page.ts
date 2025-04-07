import type { Card } from '$lib/types';
import type { PageLoad } from './$types';

export const load: PageLoad = async ({ fetch, params }) => {
    const url = "http://localhost:5124";
    const res = await fetch(url + "/deck/" + params.deck);
    const cards: Array<Card> = await res.json();

    return { 
        cards
    };
};