import { Navigate, Outlet, useLocation } from "react-router-dom";
import { Spinner } from "../../shared/ui";
import { useAuth } from "./useAuth";

export function PublicOnly() {
  const { status } = useAuth();
  const location = useLocation();
  const rawFrom = (location.state as { from?: string } | null)?.from ?? "/";
  const from =
    rawFrom.startsWith("/") && !rawFrom.startsWith("//") ? rawFrom : "/";

  if (status === "loading") {
    return (
      <div className="flex min-h-svh items-center justify-center">
        <Spinner size="lg" label="Загрузка" />
      </div>
    );
  }

  if (status === "authenticated") {
    return <Navigate to={from} replace />;
  }

  return <Outlet />;
}