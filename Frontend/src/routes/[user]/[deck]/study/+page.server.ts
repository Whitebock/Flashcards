import { API_URL } from '$lib/types';
import type { Actions } from './$types';

export const actions = {
	default: async ({request}) => {
        let data = await request.formData();

        if (data.has("answer")) {
            return {
                showAnswer: data.get("answer") == "show"
            }
        }
        else if(data.has("choice")) {
            const cardId = data.get("cardId");
            await fetch(`${API_URL}/cards/${cardId}`, {
                method: 'PATCH',
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    choice: data.get("choice")
                })
            })
        }
	}
} satisfies Actions;