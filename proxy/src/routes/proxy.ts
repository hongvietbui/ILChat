import express, { Request, Response } from 'express';
import { validateDynamic } from '../middleware/validation';
import { loadGrpcClient } from '../grpc/client';

const router = express.Router();

router.post('/_proxy/:service/:method',validateDynamic, async (req: Request, res: Response) => {
  const { service, method } = req.params;
  const data = req.body;

  let client: any;

  try {
    client = loadGrpcClient(service);
  } catch (e: any) {
    res.status(404).json({ error: e.message });
    return;
  }

  const rpcMethod = client[method];
  if (!rpcMethod || typeof rpcMethod !== 'function') {
    res.status(404).json({ error: `Method ${method} not found in ${service}` });
    return;
  }

  rpcMethod.call(client, data, (err: any, result: any) => {
    if (err) {
      console.error('gRPC error:', err);
      res.status(500).json({ error: err.message });
      return;
    }
    res.json(result);
  });
});

export default router;