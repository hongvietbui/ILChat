"use server";

import { parseCookies, setCookie } from "nookies";

export async function getToken(key: string, ctx: any = null) {
  const cookies = parseCookies(ctx);
  return cookies[key] || null;
}

export async function setToken(key: string, value: string, ctx: any = null) {
  setCookie(ctx, key, value, {
    httpOnly: true,
    secure: process.env.NODE_ENV === "production",
    path: "/",
    maxAge: key === "accessToken" ? 60 * 60 : 30 * 24 * 60 * 60,
  });
}

export async function isTokenExpired(token: string): Promise<boolean> {
  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    return payload.exp * 1000 < Date.now();
  } catch {
    return true;
  }
}

//TODO: Encrypt token before storing it in localStorage or Cookies