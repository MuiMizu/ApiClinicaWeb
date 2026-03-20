<template>
  <div id="app">
    <!-- Use MainLayout only if logged in -->
    <MainLayout v-if="authState.isLoggedIn">
      <router-view />
    </MainLayout>
    
    <!-- Otherwise, just show the router-view (Login page covers screen anyway) -->
    <router-view v-else />
  </div>
</template>

<script setup>
import { authState } from './stores/auth';
import MainLayout from './components/MainLayout.vue';
</script>

<style>
/* Reset some body/app styles to fit the new layout */
body {
  margin: 0;
  padding: 0;
  background-color: #f8f9fa !important; /* Ensure the light background for dashboard */
}

#app {
  min-height: 100vh;
}
</style>

