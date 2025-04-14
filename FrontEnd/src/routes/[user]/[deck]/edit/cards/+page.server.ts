import { API_URL } from '$lib/types';
import type { Actions } from './$types';

export const actions = {
    update: async ({request}) => { 
        let data = await request.formData();
        const cardId = data.get("cardId");

        await fetch(`${API_URL}/cards/${cardId}`, {
            method: 'PUT',
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                front: data.get("front"),
                back: data.get("back")
            })
        })
    },
    remove: async ({request}) => { 
        let data = await request.formData();
        const cardId = data.get("cardId");

        await fetch(`${API_URL}/cards/${cardId}`, {
            method: 'DELETE',
            headers: { "Content-Type": "application/json" },
        })
    },
    add: async ({request}) => { 
        let data = await request.formData();
        await fetch(`${API_URL}/cards`, {
            method: 'POST',
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                deckId: data.get("deckId"),
                front: data.get("front"),
                back: data.get("back")
            })
        })
    }
} satisfies Actions;