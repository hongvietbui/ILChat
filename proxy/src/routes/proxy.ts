import * as grpc from '@grpc/grpc-js';
import { loadGrpcClient } from '../grpc/client';
import express, { Request, Response } from 'express';
import { validateDynamic } from '../middleware/validation';
import { grpcToHttpStatus } from '../utils/grpc-to-http-status';

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

  let metadata = new grpc.Metadata();
  metadata.set('x-timestamp', new Date().toISOString());

  rpcMethod.call(client, data, metadata, (err: any, result: any) => {
    if (err) {
      const httpStatus = grpcToHttpStatus(err.code);
      res.status(httpStatus).json({
        error: err.message,
        grpcCode: err.code,
        grpcDetails: err.details || null,
      });
      return;
    }

    res.json(result);
  });
});

export default router;