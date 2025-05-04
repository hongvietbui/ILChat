import { loginSchema } from "../zod-schemas/login";

export function validateLoginInput(
    data: unknown
  ): { email: string; password: string } | { error: Record<string, string[]> } {
    const parsed = loginSchema.safeParse(data);
    if (!parsed.success) {
      return { error: parsed.error.flatten().fieldErrors };
    }
    return parsed.data;
  }