"use server";

import axios from "axios";
import { refreshAccessToken } from "./auth/keycloak";
import { getToken, isTokenExpired, setToken } from "./tokenUtils";

interface FetchWithTokenOptions {
  headers?: object;
  method?: string;
  body?: any;
}

export async function fetchWithToken(
  url: string,
  options: FetchWithTokenOptions = {}
) {
  let accessToken = await getToken("accessToken") ?? "";
  const refreshToken = await getToken("refreshToken") ?? "";

  const tokenExpired = await isTokenExpired(accessToken);
  if (!accessToken || tokenExpired) {
    try {
      const tokenData = await refreshAccessToken(refreshToken);
      accessToken = tokenData.access_token;
      await setToken("accessToken", accessToken);
      if (tokenData.refresh_token) {
        await setToken("refreshToken", tokenData.refresh_token);
      }
    } catch (error) {
      if (typeof window !== "undefined") {
        window.location.href = "/login";
      }
      return Promise.reject(
        error instanceof Error ? error : new Error(String(error))
      );
    }
  }

  const headers = {
    ...options.headers,
    Authorization: `Bearer ${accessToken}`,
  };

  return axios({
    url,
    method: options.method ?? "GET",
    headers,
    data: options.body,
  });
}
