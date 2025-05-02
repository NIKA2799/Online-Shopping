// vite.config.ts  (or .js if you prefer)
import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import { dirname, resolve } from "path";
import { fileURLToPath } from "url";

const __filename = fileURLToPath(import.meta.url); // ← recreate __filename
const __dirname = dirname(__filename); // ← recreate __dirname

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      "@": resolve(__dirname, "src"),
    },
  },
});
