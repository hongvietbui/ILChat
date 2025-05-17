"use server";

import { cookies } from "next/headers";

export async function getToken(key: string): Promise<string | null> {
  const cookieStore = await cookies();
  return cookieStore.get(key)?.value ?? null;
}

export async function isTokenExpired(token: string): Promise<boolean> {
  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    return payload.exp * 1000 < Date.now();
  } catch {
    return true;
  }
}

export async function setToken(key: string, value: string) {
  const cookieStore = await cookies();
  cookieStore.set({
    name: key,
    value,
    httpOnly: true,
    secure: process.env.NODE_ENV === "production",
    path: "/",
    maxAge: key === "accessToken" ? 60 * 60 : 30 * 24 * 60 * 60,
  });
}

export async function isTokenValid(token: string | null): Promise<boolean> {
  if (!token) return false;

  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    const now = Date.now();
    const exp = payload.exp * 1000;

    return exp > now;
  } catch {
    return false;
  }
}
