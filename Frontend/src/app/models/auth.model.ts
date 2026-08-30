export interface LoginRequest {
  email   : string;
  password: string;
}

export interface RegisterRequest {
  name    : string;
  email   : string;
  password: string;
  phone   : string;
  role    : string;
}
export interface TokenResponse {
  token : string;
  email : string;
  name  : string;
  role  : string;
  expiry: string;
}
