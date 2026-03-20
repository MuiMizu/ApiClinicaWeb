<template>
  <div class="page-container">
    <div class="page-header">
      <div class="header-left">
        <button class="btn btn-outline-secondary back-btn" @click="$router.push('/seguros')">
          ← Volver
        </button>
        <h1>{{ isEdit ? 'Editar Seguro' : 'Nuevo Seguro' }}</h1>
      </div>
    </div>
    
    <div class="page-content">
      <InsuranceForm 
        :insuranceId="id" 
        @saved="handleSaved" 
      />
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import InsuranceForm from '../components/InsuranceForm.vue';

const route = useRoute();
const router = useRouter();

const id = computed(() => {
  return route.params.id ? parseInt(route.params.id) : null;
});

const isEdit = computed(() => !!id.value);

function handleSaved() {
  router.push('/seguros');
}
</script>

<style scoped>
* {
  color: black;
}

.page-container {
  padding: 24px;
}

.page-header {
  margin-bottom: 24px;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 16px;
}

.back-btn {
  padding: 6px 12px;
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
  max-width: 600px;
}
</style>
