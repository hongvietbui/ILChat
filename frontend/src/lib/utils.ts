import { twMerge } from "tailwind-merge"
import { clsx, type ClassValue } from "clsx"

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs))
}

export function getToken(key: string) {
  return localStorage.getItem(key);
}

export function setToken(key: string, value: string) {
  localStorage.setItem(key, value);
}

export function isTokenExpired(token: string):boolean {
  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    return payload.exp * 1000 < Date.now();
  } catch {
    return true;
  }
}

//TODO: Encrypt token before storing it in localStorage or Cookies