import path from 'path';
import * as fs from 'fs';
import * as grpc from '@grpc/grpc-js';
import * as protoLoader from '@grpc/proto-loader';
import { GRPC_HOST, GRPC_PORT } from '../utils/config';

const BACKEND_URL = GRPC_HOST && GRPC_PORT ? `${GRPC_HOST}:${GRPC_PORT}` : 'http://localhost:8000';
const PROTO_DIR = path.resolve(__dirname, '../../protos');

export const grpcClients: Record<string, any> = {};

export function loadGrpcClient(serviceName: string): any {
  if (grpcClients[serviceName]) return grpcClients[serviceName];

  const protoFiles = fs.readdirSync(PROTO_DIR).filter(file => file.endsWith('.proto'));

  for (const file of protoFiles) {
    const protoPath = path.join(PROTO_DIR, file);
    const packageDefinition = protoLoader.loadSync(protoPath, {
      keepCase: true,
      longs: String,
      enums: String,
      defaults: true,
      oneofs: true,
    });

    const grpcPackage = grpc.loadPackageDefinition(packageDefinition) as any;

    for (const pkgName in grpcPackage) {
      const pkg = grpcPackage[pkgName];
      if (pkg && pkg[serviceName]) {
        const client = new pkg[serviceName](
          BACKEND_URL,
          grpc.credentials.createInsecure()
        );
        grpcClients[serviceName] = client;
        return client;
      }
    }
  }

  throw new Error(`Service ${serviceName} not found in any .proto file`);
}