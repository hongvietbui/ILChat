import { Request, Response, NextFunction } from "express";
import { z, ZodSchema } from "zod";

export const createUserSchema = z.object({
  username: z.string().min(3),
  email: z.string().email(),
  firstName: z.string().min(1),
  lastName: z.string().min(1),
  password: z.string().min(6),
});

export const userValidators = {
  "UserService/CreateUser": createUserSchema,
};

export const schemaMap: Record<string, ZodSchema<any>> = {
  ...userValidators,
  // Add more mappings from other services later
};

export const validateDynamic = (
  req: Request,
  res: Response,
  next: NextFunction
) => {
  const key = `${req.params.service}/${req.params.method}`;
  const schema = schemaMap[key];
  if (schema) {
    const result = schema.safeParse(req.body);
    if (!result.success) {
      res.status(400).json({ error: result.error.errors });
      return;
    }
  }
  next();
};
