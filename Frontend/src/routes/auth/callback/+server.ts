import { getAuthConfig } from '$lib/server/auth';
import { redirect } from '@sveltejs/kit';
import type { RequestHandler } from './$types';
import * as client from 'openid-client';

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
    )

    cookies.set('session', tokens.access_token, { 
        path: '/',
        httpOnly: true,
        sameSite: 'lax'
    });

    redirect(302, '/');
};