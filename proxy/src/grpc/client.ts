import * as grpc from '@grpc/grpc-js';
import * as protoLoader from '@grpc/proto-loader';

const packageDefinition = protoLoader.loadSync(__dirname + '/../../protos/user.proto');
const proto = grpc.loadPackageDefinition(packageDefinition) as any;

export const userClient = new proto.user.UserService(
  'localhost:50051', grpc.credentials.createInsecure()
);
