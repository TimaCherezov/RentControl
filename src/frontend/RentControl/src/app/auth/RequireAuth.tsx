import { Navigate, Outlet, useLocation } from "react-router-dom";
import { Spinner } from "../../shared/ui";
import { useAuth } from "./useAuth";

export function RequireAuth() {
  const { status } = useAuth();
  const location = useLocation();

  if (status === "loading") {
    return (
      <div className="flex min-h-svh items-center justify-center">
        <Spinner size="lg" label="Загрузка" />
      </div>
    );
  }

  if (status === "unauthenticated") {
    return (
      <Navigate
        to="/login"
        replace
        state={{ from: `${location.pathname}${location.search}` }}
      />
    );
  }

  return <Outlet />;
}