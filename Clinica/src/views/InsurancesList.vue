<template>
  <div class="page-container">
    <div class="page-header">
      <h1>Gestión de Seguros</h1>
    </div>
    <div class="page-content">
      <InsuranceList />
    </div>
  </div>
</template>

<script setup>
import InsuranceList from '../components/InsuranceList.vue';
</script>

<style scoped>
.page-container {
  padding: 24px;
}

.page-header {
  margin-bottom: 24px;
}

.page-header h1 {
  margin: 0;
  font-size: 24px;
  color: #111827;
}

.page-content {
  background: white;
  border-radius: 8px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  padding: 20px;
}
</style>
