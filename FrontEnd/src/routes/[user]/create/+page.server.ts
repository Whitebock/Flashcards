import { API_URL } from '$lib/types';
import { redirect } from '@sveltejs/kit';
import type { Actions } from './$types';
import { resolveRoute } from '$app/paths';

export const actions = {
    default: async ({request, params, locals}) => {
        let data = await request.formData();
        let name = data.get("name");

        await fetch(`${API_URL}/decks`, {
            method: 'POST',
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                name: name,
                description: data.get("description")
            })
        });

        redirect(303, resolveRoute("/[user]/[deck]/edit/cards", {
            user: params.user,
            deck: data.get("name")?.toString().toLowerCase().replaceAll(' ', '_')
        }));
    }
} satisfies Actions;