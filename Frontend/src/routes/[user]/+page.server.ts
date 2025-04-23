import { API_URL, type Deck } from '$lib/types';
import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async ({ fetch, params }) => {
    const res = await fetch(`${API_URL}/users/${params.user}`);
    const user = await res.json();
    console.log(user);

    return { user };
};