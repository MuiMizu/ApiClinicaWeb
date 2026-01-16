<template>
  <form @submit.prevent="handleSubmit" class="appointment-form">
    <div class="field" style="position: relative">
      <div class="form-floating">
        <input
          id="patientSearch"
          v-model="patientSearch"
          type="text"
          class="form-control patient-search"
          placeholder="Buscar por nombre o documento..."
          @input="searchPatients"
          :disabled="loadingPatients"
        />
        <label for="patientSearch">Paciente *</label>
      </div>
      <div v-if="patientSearch && filteredPatients.length > 0" class="patient-dropdown">
        <div
          v-for="p in filteredPatients"
          :key="p.id"
          class="patient-option"
          @click="selectPatient(p)"
        >
          {{ p.firstName }} {{ p.lastName }} - {{ p.document }}
        </div>
      </div>
      <div v-if="selectedPatient" class="selected-patient">
        ✓ Seleccionado: {{ selectedPatient.firstName }} {{ selectedPatient.lastName }}
      </div>
      <p v-if="!selectedPatient && submitted" class="error-text">
        Debes seleccionar un paciente
      </p>
    </div>

    <div class="field">
      <label>Servicio *</label>
      <select 
        v-model.number="form.serviceId" 
        @change="onServiceChange" 
        required
        :disabled="loadingServices"
      >
        <option disabled value="">Selecciona un servicio</option>
        <option v-for="s in services" :key="s.id" :value="s.id">
          {{ s.name }}
        </option>
      </select>
      <p v-if="loadingServices" class="note">Cargando servicios...</p>
    </div>
    <div class="field">
      <div class="form-floating">
        <input
          id="appointmentDate"
          v-model="form.date"
          type="date"
          class="form-control"
          :min="minDate"
          required
          @change="onDateChange"
        />
        <label for="appointmentDate">Fecha *</label>
      </div>
    </div>

    <div class="field" v-if="form.serviceId && form.date">
      <label>Médico *</label>
      <select
        v-model.number="form.doctorId"
        @change="onDoctorChange"
        :disabled="loadingDoctors || !form.serviceId || !form.date"
        required
      >
        <option disabled value="">Selecciona un médico</option>
        <option v-for="d in availableDoctors" :key="d.id" :value="d.id">
          {{ d.firstName }} {{ d.lastName }}{{ d.specialty ? ` - ${d.specialty}` : '' }}
        </option>
      </select>
      <p v-if="loadingDoctors" class="note">Cargando médicos disponibles...</p>
      <p v-if="!loadingDoctors && availableDoctors.length === 0 && form.serviceId && form.date" class="note error-text">
        No hay médicos disponibles para este servicio y fecha
      </p>
    </div>

    <div class="field" v-if="form.doctorId && form.date">
      <label>Hora *</label>
      <select
        v-model="form.time"
        :disabled="loadingTimes || !form.doctorId || !form.date"
        required
      >
        <option disabled value="">Selecciona una hora</option>
        <option
          v-for="slot in availableTimes"
          :key="slot.time"
          :value="slot.time"
          :disabled="slot.availableSlots === 0"
        >
          {{ slot.time }} 
          <span v-if="slot.availableSlots === 0">(Sin cupos)</span>
          <span v-else>({{ slot.availableSlots }} {{ slot.availableSlots === 1 ? 'cupo' : 'cupos' }})</span>
        </option>
      </select>
      <p v-if="loadingTimes" class="note">Cargando horarios...</p>
      
      <div v-if="!loadingTimes && availableTimes.length > 0" class="times-list">
        <Availability
          v-for="slot in availableTimes"
          :key="slot.time"
          :time="slot.time"
          :slots="slot.availableSlots"
        />
      </div>
    </div>

    <div class="actions">
      <button 
        class="btn btn-outline-primary"
        type="submit" 
        :disabled="submitting || !isFormValid"
      >
        {{ submitting ? 'Guardando...' : 'Crear Cita' }}
      </button>
      <button 
        class="btn btn-outline-info"
        type="button" 
        @click="resetForm" 
        :disabled="submitting"
      >
        Limpiar
      </button>
      <span v-if="message" :class="messageClass">{{ message }}</span>
    </div>
  </form>
</template>

<script setup>
import { reactive, ref, computed, watch, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { createAppointment, fetchAvailableTimes } from '../api/appointments';
import { fetchServices } from '../api/services';
import { fetchAvailableDoctors } from '../api/doctors';
import { fetchPatients } from '../api/patients';
import Availability from './Availability.vue';

const router = useRouter();

const services = ref([]);
const availableDoctors = ref([]);
const availableTimes = ref([]);
const allPatients = ref([]);
const filteredPatients = ref([]);
const selectedPatient = ref(null);
const patientSearch = ref('');

const loadingServices = ref(false);
const loadingDoctors = ref(false);
const loadingTimes = ref(false);
const loadingPatients = ref(false);
const submitting = ref(false);
const message = ref('');
const submitted = ref(false);

const form = reactive({
  serviceId: '',
  doctorId: '',
  date: '',
  time: '',
});

const minDate = new Date().toISOString().split('T')[0];

const isFormValid = computed(() => {
  return selectedPatient.value && 
  form.serviceId && 
  form.date && 
  form.doctorId && 
  form.time &&
  availableTimes.value.find(t => t.time === form.time)?.availableSlots > 0;
});

const messageClass = computed(() => {
  return message.value.includes('exitosamente') ? 'success-text' : 'error-text';
});

onMounted(async () => {
  await loadServices();
  await loadAllPatients();
});

async function loadServices() {
  loadingServices.value = true;
  try {
    services.value = await fetchServices();
  } catch (err) {
    message.value = 'Error al cargar servicios';
    console.error(err);
  } finally {
    loadingServices.value = false;
  }
}

async function loadAllPatients() {
  loadingPatients.value = true;
  try {
    const result = await fetchPatients(1, 100);
    allPatients.value = result.items || [];
  } catch (err) {
    console.error('Error loading patients', err);
  } finally {
    loadingPatients.value = false;
  }
}

function searchPatients() {
  const term = patientSearch.value.toLowerCase().trim();
  if (!term) {
    filteredPatients.value = [];
    return;
  }
  filteredPatients.value = allPatients.value.filter(
    p =>
      p.firstName.toLowerCase().includes(term) ||
      p.lastName.toLowerCase().includes(term) ||
      p.document.includes(term)
  ).slice(0, 10);
}

function selectPatient(patient) {
  selectedPatient.value = patient;
  patientSearch.value = `${patient.firstName} ${patient.lastName}`;
  filteredPatients.value = [];
}

async function onServiceChange() {
  form.doctorId = '';
  availableDoctors.value = [];
  if (form.serviceId && form.date) {
    await loadDoctors();
  }
}

async function onDateChange() {
  form.doctorId = '';
  form.time = '';
  availableDoctors.value = [];
  availableTimes.value = [];
  if (form.serviceId && form.date) {
    await loadDoctors();
  }
}

async function loadDoctors() {
  if (!form.serviceId || !form.date) return;
  loadingDoctors.value = true;
  try {
    availableDoctors.value = await fetchAvailableDoctors(form.serviceId, form.date);
  } catch (err) {
    message.value = 'Error al cargar médicos';
    console.error(err);
  } finally {
    loadingDoctors.value = false;
  }
}

async function onDoctorChange() {
  form.time = '';
  availableTimes.value = [];
  if (form.doctorId && form.date) {
    await loadTimes();
  }
}

async function loadTimes() {
  if (!form.doctorId || !form.date) return;
  loadingTimes.value = true;
  try {
    availableTimes.value = await fetchAvailableTimes(form.doctorId, form.date);
  } catch (err) {
    message.value = 'Error al cargar horarios';
    console.error(err);
  } finally {
    loadingTimes.value = false;
  }
}

watch(() => form.doctorId, () => {
  if (form.doctorId && form.date) {
    loadTimes();
  }
});

watch(() => form.date, () => {
  if (form.doctorId && form.date) {
    loadTimes();
  }
});

function resetForm() {
  form.serviceId = '';
  form.doctorId = '';
  form.date = '';
  form.time = '';
  selectedPatient.value = null;
  patientSearch.value = '';
  availableDoctors.value = [];
  availableTimes.value = [];
  message.value = '';
  submitted.value = false;
}

async function handleSubmit() {
  submitted.value = true;
  if (!isFormValid.value) {
    message.value = 'Completa todos los campos correctamente';
    return;
  }

  submitting.value = true;
  message.value = '';
  
  try {
    const dateObj = new Date(form.date + 'T00:00:00');
    await createAppointment({
      patientId: selectedPatient.value.id,
      doctorId: form.doctorId,
      date: dateObj.toISOString(),
      time: form.time,
    });
    
    message.value = 'Cita creada exitosamente';
    setTimeout(() => {
      router.push('/citas');
    }, 1500);
  } catch (err) {
    if (err?.response?.status === 409) {
      message.value = 'Sin cupos disponibles en esa hora. Intenta otra hora.';
    } else {
      message.value = err?.response?.data?.message || 'Error al crear la cita';
    }
    console.error(err);
  } finally {
    submitting.value = false;
  }
}
</script>

<style scoped>
.appointment-form {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.patient-search {
  width: 100%;
}

.patient-dropdown {
  position: absolute;
  background: white;
  border: 1px solid #cbd5e1;
  border-radius: 8px;
  max-height: 200px;
  overflow-y: auto;
  z-index: 10;
  margin-top: 4px;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
  width: 100%;
}

.patient-option {
  padding: 10px 12px;
  cursor: pointer;
  border-bottom: 1px solid #f3f4f6;
}

.patient-option:hover {
  background: #f9fafb;
}

.patient-option:last-child {
  border-bottom: none;
}

.selected-patient {
  margin-top: 8px;
  padding: 10px;
  background: #d1fae5;
  border-radius: 6px;
  font-size: 14px;
  color: #065f46;
}

.times-list {
  margin-top: 12px;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  max-height: 300px;
  overflow-y: auto;
  padding: 8px;
}

.error-text {
  color: #dc2626;
  font-size: 12px;
  margin-top: 4px;
}

.success-text {
  color: #059669;
  font-size: 14px;
}
</style>

