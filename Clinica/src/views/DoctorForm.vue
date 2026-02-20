<template>
  <div class="page-container">
    <div class="page-header">
      <h1>{{ doctorId ? 'Editar Doctor' : 'Nuevo Doctor' }}</h1>
    </div>
    <div class="page-content">
      <DoctorForm :doctor-id="doctorId" @saved="onSaved" />
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import DoctorForm from '../components/DoctorForm.vue';

const route = useRoute();
const router = useRouter();

const doctorId = computed(() => {
  const id = route.params.id;
  return id ? Number(id) : null;
});

function onSaved() {
  router.push('/doctores');
}
</script>

<style scoped>
.page-container {
  padding: 24px;
}

.page-header {
  margin-bottom: 24px;
}

.btn-back {
  background: none;
  border: none;
  color: #6b7280;
  cursor: pointer;
  padding: 0;
  margin-bottom: 8px;
  font-size: 14px;
}

.btn-back:hover {
  color: #374151;
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
  max-width: 600px;
}
</style>
