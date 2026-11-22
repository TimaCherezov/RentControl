import {
  useCallback,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from "react";
import {
  getCurrentUser,
  loginRequest,
  logoutRequest,
  registerRequest,
} from "../../features/auth/api";
import {
  AUTH_UNAUTHORIZED_EVENT,
  clearAccessToken,
  setAccessToken,
} from "../../lib/authToken";
import { refreshAccessToken } from "../../lib/http";
import { AutoLoginError } from "../../lib/errors";
import type { UserResponse } from "../../types/auth";
import { AuthContext, type AuthStatus } from "./useAuth";

export function AuthProvider({ children }: { children: ReactNode }) {
  const [status, setStatus] = useState<AuthStatus>("loading");
  const [user, setUser] = useState<UserResponse | null>(null);

  useEffect(() => {
    let active = true;

    const onUnauthorized = () => {
      if (!active) return;

      clearAccessToken();
      setUser(null);
      setStatus("unauthenticated");
    };

    window.addEventListener(AUTH_UNAUTHORIZED_EVENT, onUnauthorized);

    void (async () => {
      const refreshed = await refreshAccessToken();

      if (!active) return;

      if (!refreshed) {
        setStatus("unauthenticated");
        return;
      }

      try {
        const currentUser = await getCurrentUser();

        if (!active) return;

        setUser(currentUser);
        setStatus("authenticated");
      } catch {
        if (!active) return;

        clearAccessToken();
        setStatus("unauthenticated");
      }
    })();

    return () => {
      active = false;
      window.removeEventListener(AUTH_UNAUTHORIZED_EVENT, onUnauthorized);
    };
  }, []);

  const login = useCallback(async (email: string, password: string) => {
    const tokens = await loginRequest({ email, password });
    setAccessToken(tokens.accessToken);

    try {
      const currentUser = await getCurrentUser();
      setUser(currentUser);
      setStatus("authenticated");
    } catch (error) {
      clearAccessToken();
      throw error;
    }
  }, []);

  const register = useCallback(
    async (name: string, email: string, password: string) => {
      await registerRequest({ name, email, password });

      try {
        await login(email, password);
      } catch (error) {
        throw new AutoLoginError(
          error instanceof Error
            ? error.message
            : "Не удалось выполнить вход после регистрации",
        );
      }
    },
    [login],
  );

  const logout = useCallback(async () => {
    try {
      await logoutRequest();
    } catch (error) {
      console.error("Не удалось выполнить logout", error);
    }

    clearAccessToken();
    setUser(null);
    setStatus("unauthenticated");
  }, []);

  const value = useMemo(
    () => ({ status, user, login, register, logout }),
    [status, user, login, register, logout],
  );

  return (
    <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
  );
}