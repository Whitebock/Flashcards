import { API_URL, type Deck } from '$lib/types';
import { redirect } from '@sveltejs/kit';
import type { Actions } from './$types';
import { resolveRoute } from '$app/paths';

export const actions = {
    update: async ({request, route, params}) => {
        let data = await request.formData();
        const deckId = data.get("deckId");

        await fetch(`${API_URL}/decks/${deckId}`, {
            method: 'PUT',
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                name: data.get("name"),
                description: data.get("description")
            })
        });
        redirect(303, resolveRoute(route.id, {
            user: params.user,
            deck: data.get("name")?.toString().toLowerCase().replaceAll(' ', '_')
        }));
    },
    delete: async ({request}) => {
        let data = await request.formData();
        const deckId = data.get("deckId");

        await fetch(`${API_URL}/decks/${deckId}`, {
            method: 'DELETE',
            headers: { "Content-Type": "application/json" },
        });
    },
} satisfies Actions;