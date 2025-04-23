import type { Cookies } from '@sveltejs/kit';
import type { IDToken } from 'openid-client';
import { v4 as uuidv4 } from 'uuid';

const map = new Map();

interface Session {
    access_token: string;
    id_token: IDToken;
}

export async function startSession(cookies: Cookies, data: Session): Promise<void> {
    const session_id = uuidv4();
    map.set(session_id, data);
    cookies.set('session', session_id, { 
        path: '/',
        httpOnly: true,
        //secure: true,
        sameSite: 'lax'
    });
}

export async function getSession(cookies: Cookies): Promise<Session> {
    const id = cookies.get('session');
    return map.get(id);
}

export async function deleteSession(cookies: Cookies): Promise<void> {
    const id = cookies.get('session');
    map.delete(id);
    cookies.delete('session', { path: '/' });
}