import { useState, type FormEvent } from "react";
import { Link } from "react-router-dom";
import {
  Button,
  Card,
  Container,
  Field,
  Input,
  Stack,
} from "../shared/ui";
import { useAuth } from "../app/auth/useAuth";
import { ApiError } from "../lib/errors";

export function LoginPage() {
  const { login } = useAuth();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setError(null);
    setIsSubmitting(true);

    try {
      await login(email, password);
    } catch (submitError) {
      setError(
        submitError instanceof ApiError
          ? submitError.message
          : "Не удалось войти",
      );
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Container className="flex flex-1 items-center justify-center py-8">
      <Card className="w-full max-w-md">
        <Stack gap="md">
          <h1 className="m-0 text-2xl">Вход</h1>
          <form onSubmit={handleSubmit} noValidate>
            <Stack gap="md">
              <Field label="Email" htmlFor="login-email" required>
                <Input
                  id="login-email"
                  type="email"
                  autoComplete="email"
                  value={email}
                  onChange={(event) => setEmail(event.target.value)}
                  required
                />
              </Field>
              <Field label="Пароль" htmlFor="login-password" required>
                <Input
                  id="login-password"
                  type="password"
                  autoComplete="current-password"
                  value={password}
                  onChange={(event) => setPassword(event.target.value)}
                  required
                />
              </Field>
              {error && (
                <p role="alert" className="text-sm text-danger">
                  {error}
                </p>
              )}
              <Button type="submit" isLoading={isSubmitting} fullWidth>
                Войти
              </Button>
            </Stack>
          </form>
          <p className="text-sm text-text">
            Нет аккаунта?{" "}
            <Link to="/register" className="text-accent hover:underline">
              Зарегистрироваться
            </Link>
          </p>
        </Stack>
      </Card>
    </Container>
  );
}