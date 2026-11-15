import { httpGet, httpPost } from "../../lib/http";
import type {
  LoginRequest,
  RegisterRequest,
  TokenPairResponse,
  UserResponse,
} from "../../types/auth";

export const loginRequest = (data: LoginRequest) =>
  httpPost<TokenPairResponse>("/api/auth/login", data);

export const registerRequest = (data: RegisterRequest) =>
  httpPost<UserResponse>("/api/auth/register", data);

export const logoutRequest = () => httpPost<void>("/api/auth/logout");

export const getCurrentUser = () =>
  httpGet<UserResponse>("/api/auth/current-user");