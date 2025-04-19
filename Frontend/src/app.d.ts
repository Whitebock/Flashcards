import type { UserInfoResponse } from "openid-client";

declare global {
	namespace App {
		// interface Error {}
		interface Locals {
			user?: {
				username: string;
				picture: string;
			}
		}
		// interface PageData {}
		// interface PageState {}
		// interface Platform {}
	}
}

export {};
