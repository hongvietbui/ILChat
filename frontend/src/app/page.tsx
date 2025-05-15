"use server";

import { cookies } from "next/headers";
import { redirect } from "next/navigation";

function isTokenValid(token: string): boolean {
  if(!token || token === "") {
    return false;
  }
  //TODO: Add more validation logic if needed
  return true;
}

export default async function Home() {
  const cookieStore = await cookies();
  const accessToken = cookieStore.get("accessToken")?.value ?? "";
  //TODO: Find information about if the cookieStore nessesary to use or not
  if (!isTokenValid(accessToken)) {
    redirect("/login");
  }
  
  return (
    <div>
      <h1>This is an ILChatHomePage</h1>
    </div>
  );
}
