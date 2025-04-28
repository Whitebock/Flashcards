import { error, redirect } from '@sveltejs/kit';
import type { Actions } from './$types';
import { resolveRoute } from '$app/paths';

export const actions = {
    update: async ({ request, route, params, locals }) => {
        let data = await request.formData();
        let name = data.get("name")?.toString() ?? error(400);
        let description = data.get("description")?.toString() ?? error(400);

        await locals.api.PUT('/decks/{deckId}', {
            params: {path: {deckId: params.deck}},
            body: {
                name: name,
                description: description
            }
        });
        
        redirect(303, resolveRoute(route.id, {
            user: params.user,
            deck: name.toLowerCase().replaceAll(' ', '_')
        }));
    },
    delete: async ({ params, locals }) => {

        await locals.api.DELETE('/decks/{deckId}', {params: {path: {
            deckId: params.deck
        }}});

        redirect(303, resolveRoute("/[user]", {
            user: params.user
        }));
    },
} satisfies Actions;