<template>
  <form @submit.prevent="handleSubmit" class="patient-form">
    <div class="field">
      <div class="form-floating">
        <input 
          id="firstName"
          v-model.trim="form.firstName" 
          type="text"
          class="form-control"
          placeholder="Juan"
          required
          :disabled="loading"
        />
        <label for="firstName">Nombre *</label>
      </div>
    </div>

    <div class="field">
      <div class="form-floating">
        <input 
          id="lastName"
          v-model.trim="form.lastName" 
          type="text"
          class="form-control"
          placeholder="Pérez"
          required
          :disabled="loading"
        />
        <label for="lastName">Apellido *</label>
      </div>
    </div>

    <div class="field">
      <div class="form-floating">
        <input 
          id="document"
          v-model.trim="form.document" 
          type="text"
          class="form-control"
          placeholder="DNI / ID"
          required
          :disabled="loading"
        />
        <label for="document">Documento *</label>
      </div>
    </div>

    <div class="field">
      <div class="form-floating">
        <input 
          id="phone"
          v-model.trim="form.phone" 
          type="text"
          class="form-control"
          placeholder="+1 999 999 9999"
          :disabled="loading"
        />
        <label for="phone">Teléfono</label>
      </div>
    </div>

    <div class="field">
      <div class="form-floating">
        <input 
          id="email"
          v-model.trim="form.email" 
          type="email" 
          class="form-control"
          placeholder="correo@demo.com"
          :disabled="loading"
        />
        <label for="email">Email</label>
      </div>
    </div>

    <div class="field">
      <label>Seguro Médico *</label>
      <select 
        v-model.number="form.insuranceId" 
        required
        :disabled="loading || insurances.length === 0"
      >
        <option disabled value="">Selecciona un seguro</option>
        <option v-for="item in insurances" :key="item.id" :value="item.id">
          {{ item.name }}
        </option>
      </select>
      <p v-if="insurances.length === 0 && !loading" class="note error">
        No se pudieron cargar los seguros
      </p>
    </div>

    <div class="actions">
      <button 
      class="btn btn-outline-primary" 
        type="submit" 
        :disabled="submitting || loading"
      >
        {{ submitting ? 'Guardando...' : isEdit ? 'Actualizar' : 'Crear' }}
      </button>
      <button 
        class="btn btn-outline-info" 
        type="button" 
        @click="resetForm" 
        :disabled="submitting || loading"
      >
        Limpiar
      </button>
      <span v-if="message" :class="messageClass">{{ message }}</span>
    </div>
  </form>
</template>

<script setup>
import { computed, onMounted, reactive, ref } from 'vue';
import { createPatient, updatePatient, fetchPatientById } from '../api/patients';
import { fetchInsurances } from '../api/insurances';

const props = defineProps({
  patientId: { type: Number, default: null },
});

const emit = defineEmits(['saved']);

const insurances = ref([]);
const submitting = ref(false);
const loading = ref(false);
const message = ref('');

const form = reactive({
  firstName: '',
  lastName: '',
  document: '',
  phone: '',
  email: '',
  insuranceId: '',
});

const isEdit = computed(() => !!props.patientId);

const messageClass = computed(() => {
  return message.value.includes('Error') || message.value.includes('error') 
    ? 'error-text' 
    : 'success-text';
});

onMounted(async () => {
  try {
    insurances.value = await fetchInsurances();
    if (props.patientId) {
      await loadPatient();
    }
  } catch (err) {
    message.value = 'Error al cargar los datos';
    console.error(err);
  }
});

async function loadPatient() {
  loading.value = true;
  try {
    const patient = await fetchPatientById(props.patientId);
    form.firstName = patient.firstName;
    form.lastName = patient.lastName;
    form.document = patient.document;
    form.phone = patient.phone || '';
    form.email = patient.email || '';
    form.insuranceId = patient.insuranceId;
  } catch (err) {
    message.value = 'Error al cargar el paciente';
    console.error(err);
  } finally {
    loading.value = false;
  }
}

function resetForm() {
  if (isEdit.value) {
    loadPatient();
  } else {
    form.firstName = '';
    form.lastName = '';
    form.document = '';
    form.phone = '';
    form.email = '';
    form.insuranceId = '';
  }
  message.value = '';
}

async function handleSubmit() {
  submitting.value = true;
  message.value = '';
  
  try {
    const payload = {
      firstName: form.firstName,
      lastName: form.lastName,
      document: form.document,
      phone: form.phone || null,
      email: form.email || null,
      insuranceId: Number(form.insuranceId),
    };
    
    if (props.patientId) {
      await updatePatient(props.patientId, payload);
      message.value = 'Paciente actualizado exitosamente';
    } else {
      await createPatient(payload);
      message.value = 'Paciente creado exitosamente';
      resetForm();
    }
    
    setTimeout(() => {
      emit('saved');
    }, 1500);
  } catch (err) {
    message.value = err?.response?.data?.message || 'Error al guardar el paciente';
    console.error(err);
  } finally {
    submitting.value = false;
  }
}
</script>

<style scoped>
.patient-form {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.error-text {
  color: #dc2626;
  font-size: 14px;
}

.success-text {
  color: #059669;
  font-size: 14px;
}

.field select, option{
  color: #000000;
}
</style>

