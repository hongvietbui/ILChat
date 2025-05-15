import * as grpc from "@grpc/grpc-js";
import { loadGrpcClient } from "../grpc/client";
import express, { Request, Response } from "express";
import { validateDynamic } from "../middleware/validation";
import { grpcToHttpStatus } from "../utils/grpc-to-http-status";

const ALLOWED_HEADERS = [
  "authorization",
  "x-request-id",
  "user-agent",
  "x-custom-header",
];
const RESTFUL_METHODS = ["GET", "POST", "PUT", "DELETE", "PATCH"];

const router = express.Router();

RESTFUL_METHODS.forEach((restfulMethod) => {
  (router as any)[restfulMethod.toLowerCase()](
    "/_proxy/:service/:method",
    validateDynamic,
    async (req: Request, res: Response) => {
      const { service, method } = req.params;
      let data;
      if (restfulMethod === "GET") {
        data = req.query;
      } else {
        data = req.body;
      }

      let client: any;

      try {
        client = loadGrpcClient(service);
      } catch (e: any) {
        res.status(404).json({ error: e.message });
        return;
      }

      const rpcMethod = client[method];
      if (!rpcMethod || typeof rpcMethod !== "function") {
        res
          .status(404)
          .json({ error: `Method ${method} not found in ${service}` });
        return;
      }

      let metadata = new grpc.Metadata();
      metadata.set("x-timestamp", new Date().toISOString());

      ALLOWED_HEADERS.forEach((header) => {
        if (req.headers[header]) {
          metadata.set(header, req.headers[header] as string);
        }
      });

      console.log(req.headers);

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
    }
  );
});

export default router;
