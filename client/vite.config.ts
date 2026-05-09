import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  base: './',
  build: {
    outDir: '../api/wwwroot',
    emptyOutDir: true,
  },
  server: {
    allowedHosts: ['fanning-overbite-implement.ngrok-free.dev'],
  },
})

