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

export function RegisterPage() {
  const { register } = useAuth();

  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setError(null);
    setIsSubmitting(true);

    try {
      await register(name, email, password);
    } catch (submitError) {
      setError(
        submitError instanceof ApiError
          ? submitError.message
          : "Не удалось зарегистрироваться",
      );
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Container className="flex flex-1 items-center justify-center py-8">
      <Card className="w-full max-w-md">
        <Stack gap="md">
          <h1 className="m-0 text-2xl">Регистрация</h1>
          <form onSubmit={handleSubmit} noValidate>
            <Stack gap="md">
              <Field label="Имя" htmlFor="register-name" required>
                <Input
                  id="register-name"
                  autoComplete="name"
                  value={name}
                  onChange={(event) => setName(event.target.value)}
                  required
                />
              </Field>
              <Field label="Email" htmlFor="register-email" required>
                <Input
                  id="register-email"
                  type="email"
                  autoComplete="email"
                  value={email}
                  onChange={(event) => setEmail(event.target.value)}
                  required
                />
              </Field>
              <Field label="Пароль" htmlFor="register-password" required>
                <Input
                  id="register-password"
                  type="password"
                  autoComplete="new-password"
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
                Зарегистрироваться
              </Button>
            </Stack>
          </form>
          <p className="text-sm text-text">
            Уже есть аккаунт?{" "}
            <Link to="/login" className="text-accent hover:underline">
              Войти
            </Link>
          </p>
        </Stack>
      </Card>
    </Container>
  );
}