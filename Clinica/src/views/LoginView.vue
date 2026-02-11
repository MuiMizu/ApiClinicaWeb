<template>
  <div class="login-wrapper">
    <div class="login-container">
      <div class="card glass-card">
        <div class="card-body p-4 p-md-5">
          <div class="text-center mb-4">
            <h2 class="auth-title">Bienvenido</h2>
            <p class="text-muted">Inicia sesión para continuar</p>
          </div>
          
          <form @submit.prevent="handleLogin">
            <div class="mb-4 input-group-dynamic">
              <label for="username" class="form-label">Usuario</label>
              <input 
                type="text" 
                class="form-control glass-input" 
                id="username" 
                v-model="username" 
                placeholder="Ingresa tu usuario"
                required
              >
            </div>
            
            <div class="mb-4 input-group-dynamic">
              <label for="password" class="form-label">Contraseña</label>
              <input 
                type="password" 
                class="form-control glass-input" 
                id="password" 
                v-model="password" 
                placeholder="Ingresa tu contraseña"
                required
              >
            </div>
            
            <div v-if="error" class="alert alert-danger glass-alert mb-3" role="alert">
              {{ error }}
            </div>

            <button type="submit" class="btn btn-primary glass-btn w-100 py-2 mb-3" :disabled="loading">
              <span v-if="loading" class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
              {{ loading ? 'Iniciando...' : 'Iniciar Sesión' }}
            </button>
          </form>

          <div class="manual-token-section mt-4 pt-3 border-top border-light-subtle">
            <label class="form-label small text-muted mb-2">Ingresar Token Manualmente</label>
            <div class="input-group">
              <input 
                type="text" 
                class="form-control form-control-sm glass-input" 
                placeholder="Pegar token JWT aquí" 
                v-model="manualToken"
              >
              <button 
                class="btn btn-outline-secondary btn-sm glass-btn-secondary" 
                type="button" 
                @click="saveManualToken"
              >
                Guardar
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import axios from 'axios';
import { login } from '../stores/auth';

const router = useRouter();
const username = ref('');
const password = ref('');
const manualToken = ref('');
const error = ref('');
const loading = ref(false);

const handleLogin = async () => {
  loading.value = true;
  error.value = '';
  
  try {
    // Usamos la ruta relativa /api/... gracias al proxy configurado en vite.config.js
    const response = await axios.post('/api/auth/login', {
      username: username.value,
      password: password.value
    });

    if (response.data && response.data.token) {
      login(response.data.token);
      router.push('/pacientes');
    } else {
      error.value = 'Respuesta inesperada del servidor';
    }
  } catch (err) {
    if (err.response && err.response.status === 401) {
      error.value = 'Credenciales inválidas';
    } else {
      error.value = 'Error de conexión con el servidor. Verifica que el backend esté corriendo.';
    }
    console.error('Login error:', err);
  } finally {
    loading.value = false;
  }
};

const saveManualToken = () => {
  if (manualToken.value.trim()) {
    login(manualToken.value.trim());
    router.push('/pacientes');
  }
};
</script>

<style scoped>
.login-wrapper {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  z-index: 1000;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 1rem;
  overflow-y: auto;
}

.login-container {
  width: 100%;
  max-width: 450px;
}

.auth-title {
  font-weight: 700;
  color: #333;
  margin-bottom: 0.5rem;
}

/* Glassmorphism Card */
.glass-card {
  background: rgba(255, 255, 255, 0.9);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 16px;
  box-shadow: 0 8px 32px 0 rgba(31, 38, 135, 0.37);
}

/* Glass Inputs */
.glass-input {
  background: rgba(255, 255, 255, 0.5);
  border: 1px solid rgba(0, 0, 0, 0.1);
  border-radius: 8px;
  padding: 0.75rem 1rem;
  transition: all 0.3s ease;
}

.glass-input:focus {
  background: rgba(255, 255, 255, 0.8);
  border-color: #764ba2;
  box-shadow: 0 0 0 0.2rem rgba(118, 75, 162, 0.25);
}

/* Glass Buttons */
.glass-btn {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border: none;
  border-radius: 8px;
  font-weight: 600;
  letter-spacing: 0.5px;
  box-shadow: 0 4px 6px rgba(50, 50, 93, 0.11), 0 1px 3px rgba(0, 0, 0, 0.08);
  transition: transform 0.2s, box-shadow 0.2s;
}

.glass-btn:hover {
  transform: translateY(-1px);
  box-shadow: 0 7px 14px rgba(50, 50, 93, 0.1), 0 3px 6px rgba(0, 0, 0, 0.08);
  filter: brightness(1.1);
}

.glass-btn:active {
  transform: translateY(1px);
}

.glass-btn:disabled {
  opacity: 0.7;
  transform: none;
}

.glass-alert {
  background: rgba(220, 53, 69, 0.1);
  border: 1px solid rgba(220, 53, 69, 0.2);
  color: #dc3545;
  border-radius: 8px;
  font-size: 0.9rem;
}

.glass-btn-secondary {
  border-radius: 0 8px 8px 0;
}

.input-group-dynamic label {
  font-weight: 500;
  color: #555;
  margin-bottom: 0.5rem;
}
</style>
