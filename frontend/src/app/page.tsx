"use server";

import { cookies } from "next/headers";
import { redirect } from "next/navigation";
import { isTokenValid } from "@/lib/tokenUtils";

export default async function Home() {
  const cookieStore = await cookies();
  const accessToken = cookieStore.get("accessToken")?.value ?? "";
  if (!(await isTokenValid(accessToken))) {
    redirect("/login");
  }
  
  return (
    <div>
      <h1>This is an ILChatHomePage</h1>
    </div>
  );
}
