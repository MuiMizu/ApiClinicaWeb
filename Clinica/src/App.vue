<template>
  <div id="app">
    <MainLayout v-if="authState.isLoggedIn">
      <router-view />
    </MainLayout>
    
    <router-view v-else />
  </div>
</template>

<script setup>
import { authState } from './stores/auth';
import MainLayout from './components/MainLayout.vue';
</script>

<style>
body {
  margin: 0;
  padding: 0;
  background-color: #f8f9fa !important;
}

#app {
  min-height: 100vh;
}
</style>

