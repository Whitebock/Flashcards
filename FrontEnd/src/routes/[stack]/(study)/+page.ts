import type { CardStack } from '$lib/types';
import type { PageLoad } from './$types';

export const load: PageLoad = async ({ fetch, params }) => {
    const url = "http://localhost:5124";
    const res = await fetch(url + "/cardstacks");
    const stacks: Array<CardStack> = await res.json();
    const stack = stacks.find((stack) => stack.id === params.stack);
    if (!stack) {
        throw new Error(`Stack ${params.stack} not found`);
    }

    return { 
        cards: stack.cards
    };
};