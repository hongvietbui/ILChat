import axios from "axios";
import { LoginResult } from "@/types/auth";

const KEYCLOAK_BASE_URL = process.env.NEXT_PUBLIC_KEYCLOAK_BASE_URL;
const KEYCLOAK_REALM = process.env.NEXT_PUBLIC_KEYCLOAK_REALM;
const KEYCLOAK_CLIENT_ID = process.env.NEXT_PUBLIC_KEYCLOAK_CLIENT_ID;
const KEYCLOAK_CLIENT_SECRET = process.env.NEXT_PUBLIC_KEYCLOAK_CLIENT_SECRET;

const GRANT_TYPE = "password";

function getTokenEndpointUrl(): string {
    const baseUrl = process.env.KEYCLOAK_BASE_URL!;
    const realm = process.env.KEYCLOAK_REALM!;
    return `${baseUrl}/realms/${realm}/protocol/openid-connect/token`;
}

export async function requestKeycloakToken(email: string, password: string, rememberMe?: boolean): Promise<LoginResult> {
  const clientId = process.env.KEYCLOAK_CLIENT_ID!;
  const clientSecret = process.env.KEYCLOAK_CLIENT_SECRET!;

  const params = new URLSearchParams();
  params.append("grant_type", GRANT_TYPE);
  params.append("client_id", clientId);
  params.append("client_secret", clientSecret);
  params.append("username", email);
  params.append("password", password);

  if (rememberMe) {
    params.append("scope", "openid offline_access");
  } else {
    params.append("scope", "openid");
  }

  try {
    const response = await fetch(getTokenEndpointUrl(), {
      method: "POST",
      headers: { "Content-Type": "application/x-www-form-urlencoded" },
      body: params,
    });

    if (!response.ok) {
      const error = await response.json();
      return {
        success: false,
        error: {
          password: [error.error_description ?? "Wrong username or password"],
        },
      };
    }

    const token = await response.json();
    return {
      success: true,
      token,
    };
  } catch (err) {
    console.error("Error requesting Keycloak token:", err);
    return {
      success: false,
      error: {
        password: ["Error connecting to authentication server"]
      },
    };
  }
}

export async function refreshAccessToken(refreshToken: string) {
  const tokenRequestUrl = `${KEYCLOAK_BASE_URL}/realms/${KEYCLOAK_REALM}/protocol/openid-connect/token`;

  try {
    const response = await axios.post(
      tokenRequestUrl,
      new URLSearchParams({
        client_id: KEYCLOAK_CLIENT_ID ?? "",
        grant_type: "refresh_token",
        client_secret: KEYCLOAK_CLIENT_SECRET ?? "",
        refresh_token: refreshToken,
      }),
      {
        headers: {
          "Content-Type": "application/x-www-form-urlencoded",
        },
      }
    );

    return response.data;
  } catch (error) {
    console.error("Error refreshing access token:", error);
    throw new Error("Failed to refresh access token");
  }
}