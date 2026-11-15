import { AppLayout } from "../app/AppLayout";
import { PublicOnly } from "../app/auth/PublicOnly";
import { RequireAuth } from "../app/auth/RequireAuth";
import { HomePage } from "../pages/HomePage";
import { LoginPage } from "../pages/LoginPage";
import { RegisterPage } from "../pages/RegisterPage";
import { BookingsPage } from "../pages/BookingsPage";
import { BookingDetailPage } from "../pages/BookingDetailPage";
import { CreateBookingPage } from "../pages/CreateBookingPage";
import { ProductsPage } from "../pages/ProductsPage";
import { ProductDetailPage } from "../pages/ProductDetailPage";

export const routes = [
  {
    element: <AppLayout />,
    children: [
      {
        element: <PublicOnly />,
        children: [
          { path: "login", element: <LoginPage /> },
          { path: "register", element: <RegisterPage /> },
        ],
      },
      {
        element: <RequireAuth />,
        children: [
          { index: true, element: <HomePage /> },
          { path: "products", element: <ProductsPage /> },
          { path: "products/:id", element: <ProductDetailPage /> },
          { path: "bookings", element: <BookingsPage /> },
          { path: "bookings/new", element: <CreateBookingPage /> },
          { path: "bookings/:id", element: <BookingDetailPage /> },
        ],
      },
    ],
  },
];