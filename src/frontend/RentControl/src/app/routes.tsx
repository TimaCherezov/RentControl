import { HomePage } from "../pages/HomePage";
import { BookingsPage } from "../pages/BookingsPage";
import { BookingDetailPage } from "../pages/BookingDetailPage";
import { ProductsPage } from "../pages/ProductsPage";
import { ProductDetailPage } from "../pages/ProductDetailPage";

export const routes = [
  { path: "/", element: <HomePage /> },
  { path: "/bookings", element: <BookingsPage /> },
  { path: "/bookings/:id", element: <BookingDetailPage /> },
  { path: "/products/", element: <ProductsPage /> },
  { path: "/products/:id", element: <ProductDetailPage /> },
];
