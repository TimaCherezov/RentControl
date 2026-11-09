import { ApiError } from "./errors";

const baseUrl = "";

export async function httpGet<T>(
  url: string,
  init: RequestInit,
  query?: Record<string, unknown>,
): Promise<T> {
  const qs = new URLSearchParams();
  if (query) {
    for (const [k, v] of Object.entries(query)) {
      if (k !== undefined) qs.append(k, String(v));
    }
  }
  const res = await fetch(`${baseUrl}${url}?${qs}`, {
    ...init,
    method: "GET",
  });
  if (!res.ok) throw new ApiError(res.status, await res.text());
  return res.json() as Promise<T>;
}

export async function httpPost<T>(
  url: string,
  init: RequestInit,
  body?: unknown,
): Promise<T | void> {
  const res = await fetch(`${baseUrl}${url}`, {
    ...init,
    method: "POST",
    body: body !== undefined ? JSON.stringify(body) : undefined,
    headers: {
      "Content-Type": "application/json",
      ...init?.headers,
    },
  });
  if (!res.ok) throw new ApiError(res.status, await res.text());
  const isEmptyBody = res.headers.get("content-length") === "0";
  const isNoContent = res.status === 204;
  if (isNoContent || isEmptyBody) return;
  return res.json() as Promise<T>;
}

export async function httpPut(
  url: string,
  init: RequestInit,
  body?: unknown,
): Promise<void> {
  const res = await fetch(`${baseUrl}${url}`, {
    ...init,
    method: "PUT",
    body: body !== undefined ? JSON.stringify(body) : undefined,
    headers: {
      "Content-Type": "application/json",
      ...init?.headers,
    },
  });
  if (!res.ok) throw new ApiError(res.status, await res.text());
  return;
}

export async function httpDelete(
  url: string,
  init: RequestInit,
): Promise<void> {
  const res = await fetch(`${baseUrl}${url}`, {
    ...init,
    headers: { ...init?.headers },
  });
  if (!res.ok) throw new ApiError(res.status, await res.text());
  return;
}
