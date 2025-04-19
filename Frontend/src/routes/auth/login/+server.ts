import { getAuthConfig } from '$lib/server/auth';
import { redirect } from '@sveltejs/kit';
import type { RequestHandler } from './$types';
import * as client from 'openid-client';

export const GET: RequestHandler = async ({ cookies }) => {
    const config = await getAuthConfig();
    
    let code_verifier: string = client.randomPKCECodeVerifier()
    let code_challenge: string =  await client.calculatePKCECodeChallenge(code_verifier)
    cookies.set('code_verifier', code_verifier, { path: '/auth' });

    let parameters: Record<string, string> = {
        redirect_uri: "http://localhost:5173/auth/callback",
        scope: "openid profile email",
        code_challenge,
        code_challenge_method: 'S256',
    };

    if (!config.serverMetadata().supportsPKCE()) {
        let state = client.randomState()
        parameters.state = state
        cookies.set('state', state, { path: '/auth' });
    }

    let redirectTo: URL = client.buildAuthorizationUrl(config, parameters)
    redirect(302, redirectTo.href);
};