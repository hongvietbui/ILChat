import { Request, Response, NextFunction } from 'express';
import { z } from 'zod';

export const createUserSchema = z.object({
  username: z.string().min(3),
  email: z.string().email(),
});

export const validate = (schema: z.ZodSchema<any>) => 
  (req: Request, res: Response, next: NextFunction): void => {
    const result = schema.safeParse(req.body);
    if (!result.success) {
      res.status(400).json({ error: result.error.errors });
      return;
    }
    next();
  };
