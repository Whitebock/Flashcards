import type { paths } from "$lib/api/schema";
import type { Client } from "openapi-fetch";
import type { UserInfoResponse } from "openid-client";

declare global {
	namespace App {
		// interface Error {}
		interface Locals {
			user?: {
				id: string;
				username: string;
				picture: string;
			},
			api: Client<paths>
		}
		// interface PageData {}
		// interface PageState {}
		// interface Platform {}
	}
}

export {};
