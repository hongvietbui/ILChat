import express from 'express';
import { userClient } from '../grpc/client';
import { authenticate } from '../middleware/auth';
import { validate, createUserSchema } from '../middleware/validate';

const router = express.Router();

router.post(
  '/create',
  authenticate,
  validate(createUserSchema),
  (req, res) => {
    const { username, email } = req.body;
    userClient.CreateUser({ username, email }, (err: any, response: any) => {
      if (err) return res.status(500).json({ error: err.message });
      res.json(response);
    });
  }
);

export default router;
