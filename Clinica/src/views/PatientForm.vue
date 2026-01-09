<template>
  <main>
    <section class="card">
      <header>
        <h1>{{ isEdit ? 'Editar Paciente' : 'Nuevo Paciente' }}</h1>
        <p class="subtitle">
          {{ isEdit ? 'Modifica los datos del paciente' : 'Registra un nuevo paciente en el sistema' }}
        </p>
      </header>
      <PatientFormComponent :patient-id="patientId" @saved="handleSaved" />
    </section>
  </main>
</template>

<script setup>
import { computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import PatientFormComponent from '../components/PatientForm.vue';

const route = useRoute();
const router = useRouter();

const patientId = computed(() => route.params.id ? parseInt(route.params.id) : null);
const isEdit = computed(() => !!patientId.value);

function handleSaved() {
  router.push('/pacientes');
}
</script>

