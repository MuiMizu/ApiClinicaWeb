<template>
  <div id="app">
    <nav class="navbar">
      <div class="nav-content">
        <h1 class="nav-title">Sistema de Clínica</h1>
        <div class="nav-links">
          <router-link to="/pacientes" class="btn btn-outline-primary">Pacientes</router-link>
          <router-link to="/citas" class="btn btn-outline-primary">Citas</router-link>
        </div>
      </div>
    </nav>
    <main>
      <router-view />
    </main>
  </div>
</template>
<script setup>
</script>

