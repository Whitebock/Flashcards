import { getAuthConfig } from '$lib/server/auth';
import { redirect } from '@sveltejs/kit';
import type { RequestHandler } from './$types';
import * as client from 'openid-client';
import { startSession } from '$lib/server/session';

export const GET: RequestHandler = async ({ url, cookies }) => {
    const config = await getAuthConfig();

    const code_verifier = cookies.get('code_verifier');
    const state = cookies.get('state');

    let tokens = await client.authorizationCodeGrant(
        config,
        url,
        {
            pkceCodeVerifier: code_verifier,
            expectedState: state,
        },
        {},
    );

    cookies.delete("code_verifier", {path: "/auth"});
    cookies.delete('state', { path: '/auth' });

    await startSession(cookies, {
        access_token: tokens.access_token,
        id_token: tokens.claims()!
    });

    redirect(302, '/');
};