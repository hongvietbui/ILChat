import { status as GrpcStatus } from '@grpc/grpc-js';

export const grpcToHttpStatus = (grpcCode: number): number => {
    switch (grpcCode) {
      case GrpcStatus.OK: return 200;
      case GrpcStatus.INVALID_ARGUMENT: return 400;
      case GrpcStatus.NOT_FOUND: return 404;
      case GrpcStatus.ALREADY_EXISTS: return 409;
      case GrpcStatus.PERMISSION_DENIED: return 403;
      case GrpcStatus.UNAUTHENTICATED: return 401;
      case GrpcStatus.RESOURCE_EXHAUSTED: return 429;
      case GrpcStatus.UNIMPLEMENTED: return 501;
      case GrpcStatus.UNAVAILABLE: return 503;
      case GrpcStatus.DEADLINE_EXCEEDED: return 504;
      case GrpcStatus.INTERNAL: return 500;
      case GrpcStatus.UNKNOWN: return 404;
      default:
        return 500;
    }
  };
  