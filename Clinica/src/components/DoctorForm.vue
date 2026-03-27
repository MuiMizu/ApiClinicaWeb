<template>
  <form @submit.prevent="handleSubmit" class="doctor-form">
    <div class="field">
      <div class="form-floating">
        <input 
          id="firstName"
          v-model.trim="form.firstName" 
          @input="form.firstName = form.firstName.replace(/[^a-zA-ZáéíóúÁÉÍÓÚñÑ\s]/g, '')"
          maxlength="32"
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
          @input="form.lastName = form.lastName.replace(/[^a-zA-ZáéíóúÁÉÍÓÚñÑ\s]/g, '')"
          maxlength="32"
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
        <select 
          id="specialty"
          v-model.number="form.serviceId" 
          class="form-select"
          required
          :disabled="loading"
        >
          <option value="" disabled>Seleccione un servicio</option>
          <option v-for="service in availableServices" :key="service.id" :value="service.id">
            {{ service.name }}
          </option>
        </select>
        <label for="specialty">Servicio *</label>
      </div>
    </div>


    <div class="field">
      <div class="form-floating">
        <input 
          id="phone"
          v-model.trim="form.phone" 
          @input="form.phone = form.phone.replace(/[^0-9+ ]/g, '')"
          maxlength="16"
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
import { fetchServices } from '../api/services';

const props = defineProps({
  doctorId: { type: Number, default: null },
});

const emit = defineEmits(['saved']);

const availableServices = ref([]);
const submitting = ref(false);
const loading = ref(false);
const message = ref('');
const isError = ref(false);

const form = reactive({
  firstName: '',
  lastName: '',
  serviceId: '',
  phone: '',
  email: '',
});

const isEdit = computed(() => !!props.doctorId);

const messageClass = computed(() => {
  return isError.value ? 'error-text' : 'success-text';
});

onMounted(async () => {
  await loadServices();
  if (props.doctorId) {
    await loadDoctor();
  }
});

async function loadServices() {
  try {
    const result = await fetchServices();
    availableServices.value = Array.isArray(result) ? result : (result.items || []);
  } catch (err) {
    console.error('Error al cargar servicios:', err);
  }
}

async function loadDoctor() {
  loading.value = true;
  try {
    const doctor = await fetchDoctorById(props.doctorId);
    form.firstName = doctor.firstName;
    form.lastName = doctor.lastName;
    // Pre-select the first associated service if any
    form.serviceId = (doctor.serviceIds && doctor.serviceIds.length > 0)
      ? doctor.serviceIds[0]
      : '';
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
    form.serviceId = '';
    form.phone = '';
    form.email = '';
  }
  message.value = '';
}

async function handleSubmit() {
  submitting.value = true;
  message.value = '';
  isError.value = false;
  
  try {
    const selectedService = availableServices.value.find(s => s.id === form.serviceId);
    const payload = {
      firstName: form.firstName,
      lastName: form.lastName,
      specialty: selectedService?.name || null,
      serviceIds: form.serviceId ? [form.serviceId] : [],
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
    isError.value = true;
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

.actions {
  display: flex;
  gap: 12px;
  align-items: center;
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
