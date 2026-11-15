import { useNavigate } from "react-router-dom";
import { Button, Container, Stack } from "../shared/ui";

export function HomePage() {
  const navigate = useNavigate();

  return (
    <Container className="py-12">
      <Stack gap="lg" className="max-w-2xl">
        <h1>RentControl</h1>
        <p className="text-lg text-text">
          Сервис для бронирования инструментов и оборудования: выберите
          продукт, посмотрите свободные даты и оформите бронь за пару минут.
        </p>
        <div>
          <Button onClick={() => navigate("/products")}>К каталогу</Button>
        </div>
      </Stack>
    </Container>
  );
}