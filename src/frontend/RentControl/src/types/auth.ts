export type TokenPairResponse = {
  accessToken: string;
  refreshToken: string;
};

export type UserResponse = {
  id: string;
  name: string;
  email: string;
};

export type LoginRequest = {
  email: string;
  password: string;
};

export type RegisterRequest = {
  name: string;
  email: string;
  password: string;
};