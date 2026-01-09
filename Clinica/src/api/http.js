import axios from 'axios';

const http = axios.create({
  // Usar URL relativa para aprovechar el proxy de Vite en desarrollo
  // En producción, usar la variable de entorno VITE_API_URL
  baseURL: import.meta.env.VITE_API_URL || '/api',
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Interceptor para manejar errores
http.interceptors.response.use(
  (response) => response,
  (error) => {
    console.error('API Error:', error.response?.data || error.message);
    return Promise.reject(error);
  }
);

export default http;

