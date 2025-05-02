import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import path from "path"; // ← add Node ‘path’

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "src"), // now "@/..." points to /src
    },
  },
});
