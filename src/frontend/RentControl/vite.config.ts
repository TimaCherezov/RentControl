import tailwindcss from "@tailwindcss/vite";
import react from "@vitejs/plugin-react";
import { defineConfig } from "vite";

// https://vite.dev/config/
export default defineConfig({
  plugins: [react(), tailwindcss()],
  server: {
    proxy: {
      "/api/invoices": {
        target:
          process.env.VITE_PAYMENTS_API_TARGET ?? "https://localhost:7287",
        changeOrigin: true,
        secure: false,
      },
      "/api": {
        target: process.env.VITE_API_TARGET ?? "https://localhost:7275",
        changeOrigin: true,
        secure: false,
      },
    },
  },
});
