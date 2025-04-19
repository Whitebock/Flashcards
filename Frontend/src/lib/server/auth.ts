import { env } from '$env/dynamic/private';
import { Configuration, discovery } from 'openid-client';

export async function getAuthConfig(): Promise<Configuration> {
    return await discovery(
        new URL(env.OIDC_SERVER),
        env.OIDC_CLIENT_ID,
        env.OIDC_CLIENT_SECRET,
    );
}