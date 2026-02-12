<template>
  <div id="app">
    <nav class="navbar" v-if="authState.isLoggedIn">
      <div class="nav-content">
        <h1 class="nav-title">Sistema de Clínica</h1>
        <div class="nav-links">
          <router-link to="/pacientes" class="btn btn-outline-primary me-2">Pacientes</router-link>
          <router-link to="/citas" class="btn btn-outline-primary me-2">Citas</router-link>
          <button @click="handleLogout" class="btn btn-outline-danger">Cerrar Sesión</button>
        </div>
      </div>
    </nav>
    <main>
      <router-view />
    </main>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router';
import { authState, logout } from './stores/auth';

const router = useRouter();

const handleLogout = () => {
    logout();
    router.push('/login');
};
</script>

