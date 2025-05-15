"use server";

import { validateLoginInput } from "@/lib/auth/validate";
import { LoginFormValues } from "@/lib/zod-schemas/login";
import { requestKeycloakToken } from "@/lib/auth/keycloak";
import {
  LoginResult,
  RegisterUserInput,
  RegisterUserResponse,
} from "@/types/auth";

export async function loginAction(data: unknown): Promise<LoginResult> {
  const validated = validateLoginInput(data);
  if ("error" in validated) {
    return { success: false, error: validated.error };
  }

  const { email, password, rememberMe } = validated as LoginFormValues & {
    rememberMe?: boolean;
  };
  return await requestKeycloakToken(email, password, rememberMe);
}

export async function registerUser(
  input: RegisterUserInput
): Promise<RegisterUserResponse> {
  const proxyUrl = `${process.env.NEXT_PUBLIC_PROXY_URL}/_proxy/UserService/CreateUser`;

  try {
    const res = await fetch(proxyUrl, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(input),
    });

    if (!res.ok) {
      const err = await res.json().catch(() => ({}));
      return {
        success: false,
        message: err?.error ?? `HTTP ${res.status} ${res.statusText}`,
      };
    }

    const data = await res.json();
    return {
      success: true,
      data,
    };
  } catch (err: any) {
    return {
      success: false,
      message: err.message ?? "Unknown error occurred",
    };
  }
}
