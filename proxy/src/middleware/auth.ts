import { Request, Response, NextFunction } from "express";
import jwt, { JwtHeader } from "jsonwebtoken";
import jwksClient from "jwks-rsa";
import { KEYCLOAK_URL } from "../utils/config";

const client = jwksClient({
  jwksUri:
    `${KEYCLOAK_URL}/realms/YOUR_REALM/protocol/openid-connect/certs`
});

function getKey(header: JwtHeader, callback: jwt.SigningKeyCallback) {
  client.getSigningKey(header.kid!, (err, key) => {
    if (err) return callback(err);
    const signingKey = key?.getPublicKey();
    callback(null, signingKey);
  });
}

export const authenticate = (
  req: Request,
  res: Response,
  next: NextFunction
): void => {
  const authHeader = req.headers.authorization;
  const token = authHeader?.split(" ")[1];
  if (!token) {
    res.status(401).json({ error: "Missing token" });
    return;
  }

  jwt.verify(
    token,
    getKey,
    {
      algorithms: ["RS256"],
      issuer: `${KEYCLOAK_URL}/realms/ILChat`,
    },
    (err, decoded) => {
      if (err) {
        res.status(403).json({ error: "Invalid token" });
        return;
      }
      (req as any).user = decoded;
      next();
    }
  );
};
