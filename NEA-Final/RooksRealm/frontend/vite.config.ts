import { defineConfig } from "vite";
import react from "@vitejs/plugin-react-swc";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5182,
    open: "/",
    host: "0.0.0.0",
    proxy: {
      "/proxy": {
        target: "https://localhost:7204",
        changeOrigin: true,
        rewrite: (path) => path.replace(/^\/proxy/, ""),
        secure: false,
        ws: true,
      },
    },
    allowedHosts: [
      "chess.mcharg.com",
    ],
  },
});
