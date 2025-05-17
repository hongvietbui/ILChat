import { requestKeycloakToken } from "@/lib/auth/keycloak";
import { validateLoginInput } from "@/lib/auth/validate";
import { NextRequest, NextResponse } from "next/server";

export async function POST(req: NextRequest) {
  const data = await req.json();
  const validated = validateLoginInput(data);
  if ("error" in validated) {
    return NextResponse.json(
      { success: false, error: validated.error },
      { status: 400 }
    );
  }

  const {email, password, rememberMe} = validated as { email: string; password: string; rememberMe?: boolean };
  const result = await requestKeycloakToken(email, password, rememberMe);

  if (!result.success) {
    return NextResponse.json(result, { status: 401 });
  }

  const response = NextResponse.json({ success: true });
  const maxAgeAccess = 60 * 60; // 1 hour
  const maxAgeRefresh = 30 * 24 * 60 * 60; // 30 days

  response.cookies.set("accessToken", result.token.access_token, {
    httpOnly: true,
    secure: process.env.NODE_ENV === "production",
    path: "/",
    maxAge: maxAgeAccess,
  });

  if (result.token.refresh_token) {
    response.cookies.set("refreshToken", result.token.refresh_token, {
      httpOnly: true,
      secure: process.env.NODE_ENV === "production",
      path: "/",
      maxAge: maxAgeRefresh,
    });
  }

  return response;
}
