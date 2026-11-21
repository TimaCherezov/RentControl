import { Link, useNavigate } from "react-router-dom";
import { Button, Container } from "../shared/ui";
import { useAuth } from "./auth/useAuth";

export function AppHeader() {
  const navigate = useNavigate();
  const { status, user, logout } = useAuth();

  return (
    <header className="sticky top-0 z-10 border-b border-border bg-bg/95 backdrop-blur">
      <Container className="flex min-h-14 flex-wrap items-center justify-between gap-2 py-2">
        <Link to="/" className="font-medium text-text-h">
          RentControl
        </Link>

        {status === "authenticated" && (
          <div className="flex flex-wrap items-center gap-2">
            {user && (
              <span className="max-w-[200px] truncate text-sm text-text">
                {user.email}
              </span>
            )}
            <Button
              variant="ghost"
              size="sm"
              onClick={() => navigate("/products")}
            >
              Каталог
            </Button>
            <Button
              variant="ghost"
              size="sm"
              onClick={() => navigate("/bookings")}
            >
              Брони
            </Button>
            <Button
              variant="secondary"
              size="sm"
              onClick={() => void logout()}
            >
              Выйти
            </Button>
          </div>
        )}
      </Container>
    </header>
  );
}