<template>
  <form @submit.prevent="handleSubmit" class="doctor-form">
    <div class="field">
      <div class="form-floating">
        <input 
          id="firstName"
          v-model.trim="form.firstName" 
          type="text"
          class="form-control"
          placeholder="Nombre"
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
          placeholder="Apellido"
          required
          :disabled="loading"
        />
        <label for="lastName">Apellido *</label>
      </div>
    </div>

    <div class="field">
      <div class="form-floating">
        <input 
          id="specialty"
          v-model.trim="form.specialty" 
          type="text"
          class="form-control"
          placeholder="Especialidad"
          required
          :disabled="loading"
        />
        <label for="specialty">Especialidad *</label>
      </div>
    </div>

    <div class="field">
      <div class="form-floating">
        <input 
          id="licenseNumber"
          v-model.trim="form.licenseNumber" 
          type="text"
          class="form-control"
          placeholder="Número de Licencia"
          :disabled="loading"
        />
        <label for="licenseNumber">Número de Licencia</label>
      </div>
    </div>

    <div class="field">
      <div class="form-floating">
        <input 
          id="phone"
          v-model.trim="form.phone" 
          type="text"
          class="form-control"
          placeholder="Teléfono"
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
          placeholder="Email"
          :disabled="loading"
        />
        <label for="email">Email</label>
      </div>
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
import { createDoctor, updateDoctor, fetchDoctorById } from '../api/doctors';

const props = defineProps({
  doctorId: { type: Number, default: null },
});

const emit = defineEmits(['saved']);

const submitting = ref(false);
const loading = ref(false);
const message = ref('');

const form = reactive({
  firstName: '',
  lastName: '',
  specialty: '',
  licenseNumber: '',
  phone: '',
  email: '',
});

const isEdit = computed(() => !!props.doctorId);

const messageClass = computed(() => {
  return message.value.includes('Error') || message.value.includes('error') 
    ? 'error-text' 
    : 'success-text';
});

onMounted(async () => {
  if (props.doctorId) {
    await loadDoctor();
  }
});

async function loadDoctor() {
  loading.value = true;
  try {
    const doctor = await fetchDoctorById(props.doctorId);
    form.firstName = doctor.firstName;
    form.lastName = doctor.lastName;
    form.specialty = doctor.specialty || '';
    form.licenseNumber = doctor.licenseNumber || '';
    form.phone = doctor.phone || '';
    form.email = doctor.email || '';
  } catch (err) {
    message.value = 'Error al cargar el doctor';
    console.error(err);
  } finally {
    loading.value = false;
  }
}

function resetForm() {
  if (isEdit.value) {
    loadDoctor();
  } else {
    form.firstName = '';
    form.lastName = '';
    form.specialty = '';
    form.licenseNumber = '';
    form.phone = '';
    form.email = '';
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
      specialty: form.specialty,
      licenseNumber: form.licenseNumber || null,
      phone: form.phone || null,
      email: form.email || null,
    };
    
    if (props.doctorId) {
      await updateDoctor(props.doctorId, payload);
      message.value = 'Doctor actualizado exitosamente';
    } else {
      await createDoctor(payload);
      message.value = 'Doctor creado exitosamente';
      resetForm();
    }
    
    setTimeout(() => {
      emit('saved');
    }, 1500);
  } catch (err) {
    message.value = err?.response?.data?.message || 'Error al guardar el doctor';
    console.error(err);
  } finally {
    submitting.value = false;
  }
}
</script>

<style scoped>
* {
  color: black;
}

.doctor-form {
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
</style>
