import type { PageLoad } from './$types';

export const load: PageLoad = async ({ fetch }) => {
    const url = "http://localhost:5011";
	const res = await fetch(url + "/cardstacks");
	const stacks = await res.json();

	return { 
        stacks
    };
};