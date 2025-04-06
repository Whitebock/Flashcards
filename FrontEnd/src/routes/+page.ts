import type { CardStack } from '$lib/types';
import type { PageLoad } from './$types';

export const load: PageLoad = async ({ fetch }) => {
    const url = "http://localhost:5124";
	const res = await fetch(url + "/cardstacks");
	const stacks: Array<CardStack> = await res.json();

	return { 
        stacks
    };
};