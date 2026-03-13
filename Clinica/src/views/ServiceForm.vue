<template>
  <div class="page-container">
    <div class="page-header">
      <button class="btn btn-outline-info btn-sm" @click="$router.push('/servicios')">
        &larr; Volver
      </button>
      <h1>{{ serviceId ? 'Editar Servicio' : 'Nuevo Servicio' }}</h1>
    </div>
    <div class="page-content">
      <ServiceForm :service-id="serviceId" @saved="$router.push('/servicios')" />
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue';
import { useRoute } from 'vue-router';
import ServiceForm from '../components/ServiceForm.vue';

const route = useRoute();
const serviceId = computed(() => route.params.id ? Number(route.params.id) : null);
</script>

<style scoped>
.page-container {
  padding: 24px;
  max-width: 800px;
  margin: 0 auto;
}

.page-header {
  margin-bottom: 24px;
  display: flex;
  align-items: center;
  gap: 16px;
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
  padding: 24px;
}
</style>
