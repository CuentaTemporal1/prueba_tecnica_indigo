/** @type {import('tailwindcss').Config} */

// SOLUCIÓN 1: Usa "export default" en lugar de "module.exports"
export default {
  // SOLUCIÓN 2: Usa "content" en lugar de "purge"
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],
  
  // SOLUCIÓN 3: Elimina la línea "darkMode: false"
  
  theme: {
    extend: {},
  },
  plugins: [],
}