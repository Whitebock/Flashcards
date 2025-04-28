import buildApiClient from '$lib/api';
import { getAuthConfig } from '$lib/server/auth';
import { getSession } from '$lib/server/session';
import type { Handle } from '@sveltejs/kit';
import { fetchUserInfo } from 'openid-client';

export const handle: Handle = async ({ event, resolve }) => {
    
    let session = await getSession(event.cookies);
    if (session) {
        const config = await getAuthConfig();
        let {access_token, id_token} = session;
        let userInfo = await fetchUserInfo(config, access_token, id_token.sub);
        
        event.locals.user = {
            id: userInfo["app_id"]!.toString(),
            username: userInfo.preferred_username ?? userInfo.nickname!,
            picture: userInfo.picture!
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