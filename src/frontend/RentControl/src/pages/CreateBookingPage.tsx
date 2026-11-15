import { Container, Stack } from "../shared/ui";
import { CreateBookingForm } from "../features/bookings/ui/CreateBookingForm";

export function CreateBookingPage() {
  return (
    <Container className="py-8">
      <Stack gap="lg">
        <h1>Новая бронь</h1>
        <CreateBookingForm />
      </Stack>
    </Container>
  );
}