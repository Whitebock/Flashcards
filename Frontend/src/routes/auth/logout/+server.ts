import { getAuthConfig } from '$lib/server/auth';
import { redirect } from '@sveltejs/kit';
import type { RequestHandler } from './$types';
import * as client from 'openid-client';

export const GET: RequestHandler = async ({ cookies }) => {
    const config = await getAuthConfig();

    cookies.delete("code_verifier", {path: "/auth"});
    cookies.delete('state', { path: '/auth' });
    cookies.delete('session', { path: '/' });

    let redirectTo = client.buildEndSessionUrl(config, {
        post_logout_redirect_uri: "http://localhost:5173",
        //id_token_hint: id_token,
    })

    redirect(302, redirectTo.href);
};