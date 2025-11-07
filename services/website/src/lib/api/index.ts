import createClient, { type ClientOptions } from "openapi-fetch";
import { type paths } from "./schema.d";
import { API_URL } from '$env/static/private';

export default function buildApiClient(customFetch: ClientOptions["fetch"]) {
    return createClient<paths>({ baseUrl: API_URL, fetch: customFetch });
}