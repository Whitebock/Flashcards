import createClient, { type ClientOptions } from "openapi-fetch";
import { type paths } from "./schema.d";

export const API_URL = "http://localhost:5124";

export default function buildApiClient(customFetch: ClientOptions["fetch"]) {
    return createClient<paths>({ baseUrl: API_URL, fetch: customFetch });
}