import { error, redirect } from '@sveltejs/kit';
import type { Actions } from './$types';
import { resolveRoute } from '$app/paths';

export const actions = {
    default: async ({ request, params, locals }) => {
        let data = await request.formData();
        let name = data.get("name")?.toString() ?? error(400);
        let description = data.get("description")?.toString() ?? error(400);

        await locals.api.POST('/decks', {
            body: {
                name: name,
                description: description
            }
        });

        redirect(303, resolveRoute("/[user]/[deck]/edit/cards", {
            user: params.user,
            deck: name.toLowerCase().replaceAll(' ', '_')
        }));
    }
} satisfies Actions;