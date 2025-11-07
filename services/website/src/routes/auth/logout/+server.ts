import { getAuthConfig } from '$lib/server/auth';
import { redirect } from '@sveltejs/kit';
import type { RequestHandler } from './$types';
import * as client from 'openid-client';
import { deleteSession } from '$lib/server/session';

export const GET: RequestHandler = async ({ cookies }) => {

    await deleteSession(cookies);

    const config = await getAuthConfig();
    let redirectTo = client.buildEndSessionUrl(config, {
        post_logout_redirect_uri: "http://localhost:5173",
        //id_token_hint: id_token,
    })

    redirect(302, redirectTo.href);
};