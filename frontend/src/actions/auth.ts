"use server";

import { LoginResult } from "@/types/auth";
import { validateLoginInput } from "@/lib/auth/validate";
import { requestKeycloakToken } from "@/lib/auth/keycloak";

export async function loginAction(data: unknown): Promise<LoginResult> {
  const validated = validateLoginInput(data);

  if ("error" in validated) {
    return { success: false, error: validated.error };
  }

  const { email, password } = validated;
  return await requestKeycloakToken(email, password);
}