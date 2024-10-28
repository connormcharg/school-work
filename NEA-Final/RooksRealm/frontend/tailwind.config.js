/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./src/**/*.{js,jsx,ts,tsx}",
    "./index.html"
  ],
  theme: {
    extend: {
      spacing: {
        "1/8": "calc(100% / 8)",
      }
    },
  },
  plugins: [],
}