export type LoginResult =
  | { success: true; token: any }
  | { success: false; error: Record<string, string[]> };