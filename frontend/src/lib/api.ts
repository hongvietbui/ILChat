"use server";

import axios from "axios";
import { refreshAccessToken } from "./auth/keycloak";
import { getToken, isTokenExpired, setToken } from "./utils";

interface FetchWithTokenOptions {
  headers?: object;
  method?: string;
  body?: any;
}

export async function fetchWithToken(
  url: string,
  options: FetchWithTokenOptions = {}
) {
  let accessToken = getToken("accessToken") ?? "";
  const refreshToken = getToken("refreshToken") ?? "";

  if (!accessToken || isTokenExpired(accessToken)) {
    try {
      const tokenData = await refreshAccessToken(refreshToken);
      accessToken = tokenData.access_token;
      setToken("accessToken", accessToken);
      if (tokenData.refresh_token) {
        setToken("refreshToken", tokenData.refresh_token);
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
