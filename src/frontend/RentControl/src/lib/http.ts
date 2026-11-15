import { ApiError } from "./errors";
import { dropUndefined } from "../shared/utils/filter";
import {
  AUTH_UNAUTHORIZED_EVENT,
  clearAccessToken,
  getAccessToken,
  setAccessToken,
} from "./authToken";
import type { TokenPairResponse } from "../types/auth";

const refreshUrl = "/api/auth/refresh";

const authEndpoints = [
  "/api/auth/login",
  "/api/auth/register",
  "/api/auth/refresh",
  "/api/auth/logout",
];

let refreshPromise: Promise<boolean> | null = null;

async function requestRefreshToken(): Promise<boolean> {
  try {
    const res = await fetch(refreshUrl, {
      method: "POST",
      credentials: "include",
    });
    if (!res.ok) return false;

    const tokens = (await res.json()) as TokenPairResponse;
    setAccessToken(tokens.accessToken);
    return true;
  } catch {
    return false;
  }
}

export function refreshAccessToken(): Promise<boolean> {
  if (!refreshPromise) {
    refreshPromise = requestRefreshToken().finally(() => {
      refreshPromise = null;
    });
  }

  return refreshPromise;
}

async function readErrorMessage(res: Response): Promise<string> {
  const text = await res.text();

  try {
    const body = JSON.parse(text) as {
      error?: unknown;
      title?: unknown;
      errors?: unknown;
    };

    if (typeof body.error === "string") return body.error;
    if (typeof body.title === "string") return body.title;
    if (body.errors) return JSON.stringify(body.errors);
  } catch {
    return text || res.statusText;
  }

  return text || res.statusText;
}

async function request<T>(
  url: string,
  init: RequestInit = {},
  allowRefresh = true,
): Promise<T> {
  const headers = new Headers(init.headers);
  const token = getAccessToken();
  if (token) headers.set("Authorization", `Bearer ${token}`);

  const res = await fetch(url, {
    credentials: "include",
    ...init,
    headers,
  });

  const isAuthEndpoint = authEndpoints.some((path) => url.startsWith(path));

  if (res.status === 401 && allowRefresh && !isAuthEndpoint) {
    const refreshed = await refreshAccessToken();

    if (refreshed) {
      return request<T>(url, init, false);
    }

    clearAccessToken();
    window.dispatchEvent(new Event(AUTH_UNAUTHORIZED_EVENT));
  }

  if (!res.ok) throw new ApiError(res.status, await readErrorMessage(res));

  const isEmptyBody = res.headers.get("content-length") === "0";
  const isNoContent = res.status === 204 || res.status === 205;
  if (isNoContent || isEmptyBody) return undefined as T;

  return res.json() as Promise<T>;
}

function buildUrl(url: string, query?: Record<string, unknown>): string {
  if (!query) return url;
  const qs = new URLSearchParams();
  for (const [k, v] of Object.entries(dropUndefined(query))) {
    if (v !== null) qs.append(k, String(v));
  }
  const s = qs.toString();
  return s ? `${url}?${s}` : url;
}

export const httpGet = <T>(
  url: string,
  query?: Record<string, unknown>,
  init?: RequestInit,
): Promise<T> => request<T>(buildUrl(url, query), { ...init, method: "GET" });

export const httpPost = <T>(
  url: string,
  body?: unknown,
  init: RequestInit = {},
): Promise<T> =>
  request<T>(buildUrl(url), {
    ...init,
    method: "POST",
    body: body !== undefined ? JSON.stringify(body) : undefined,
    headers:
      body !== undefined
        ? { "Content-Type": "application/json", ...init?.headers }
        : init?.headers,
  });

export const httpPut = (url: string, body?: unknown, init: RequestInit = {}) =>
  request<void>(buildUrl(url), {
    ...init,
    method: "PUT",
    body: body !== undefined ? JSON.stringify(body) : undefined,
    headers:
      body !== undefined
        ? { "Content-Type": "application/json", ...init?.headers }
        : init?.headers,
  });

export const httpDelete = (url: string, init: RequestInit = {}) =>
  request<void>(buildUrl(url), { ...init, method: "DELETE" });
