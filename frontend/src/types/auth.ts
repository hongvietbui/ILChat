export type LoginResult =
  | { success: true; token: any }
  | { success: false; error: Record<string, string[]> };

export type RegisterUserInput = {
  username: string;
  email: string;
  password: string;
  firstName: string;
  lastName: string;
};

export type RegisterUserResponse = {
  success: boolean;
  message?: string;
  data?: any;
};
