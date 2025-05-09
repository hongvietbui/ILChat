import { LoginResult } from "@/types/auth";

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
          password: [error.error_description || "Sai email hoặc mật khẩu"],
        },
      };
    }

    const token = await response.json();
    return {
      success: true,
      token,
    };
  } catch (err) {
    return {
      success: false,
      error: {
        password: ["Error connecting to authentication server"]
      },
    };
  }
}