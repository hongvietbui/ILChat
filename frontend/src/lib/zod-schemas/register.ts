import { z } from "zod";

export const RegisterSchema = z
    .object({
        username: z.string().min(3, 'Username must be at least 3 characters long'),
        email: z.string().email('Invalid email address'),
        password: z.string().min(6, 'Password must be at least 6 characters long'),
        confirmPassword: z.string().min(6, 'Password must be at least 6 characters long'),
        firstName: z.string().min(1, 'Required'),
        lastName: z.string().min(1, 'Required'),
    })
    .refine((data) => data.password === data.confirmPassword, {
        message: "Passwords do not match",
        path: ["confirmPassword"],
    });

export type RegisterFormValues = z.infer<typeof RegisterSchema>;