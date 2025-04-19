import { getAuthConfig } from '$lib/server/auth';
import type { Handle } from '@sveltejs/kit';
import { fetchUserInfo, skipSubjectCheck } from 'openid-client';

export const handle: Handle = async ({ event, resolve }) => {
    const config = await getAuthConfig();
    const session = event.cookies.get('session');
    
    if (session) {
        let userInfo = await fetchUserInfo(config, session, skipSubjectCheck);
        
        event.locals.user = {
            username: userInfo.preferred_username ?? userInfo.nickname!,
            picture: userInfo.picture!
        };
    }

    return await resolve(event);
};