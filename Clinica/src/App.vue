<template>
  <div id="app">
    <nav class="navbar">
      <div class="nav-content">
        <h1 class="nav-title">Sistema de Clínica</h1>
        <div class="nav-links">
          <router-link to="/pacientes" class="nav-link">Pacientes</router-link>
          <router-link to="/citas" class="nav-link">Citas</router-link>
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

