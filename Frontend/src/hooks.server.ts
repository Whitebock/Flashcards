import buildApiClient from '$lib/api';
import { getSession } from '$lib/server/session';
import type { Handle } from '@sveltejs/kit';

export const handle: Handle = async ({ event, resolve }) => {
    
    let session = await getSession(event.cookies);
    if (session) {
        event.locals.user = {
            id: session.user_info["app_id"]!.toString(),
            username: session.user_info.preferred_username ?? session.user_info.nickname!,
            picture: session.user_info.picture!
        };
    }

    event.locals.api = buildApiClient(event.fetch);

    return await resolve(event);
};

export async function handleFetch({ event, request, fetch }) {
    let session = await getSession(event.cookies);
    if(session) {
        request.headers.set('Authorization', `Bearer ${session.access_token}`);
    }
	return await fetch(request);
}