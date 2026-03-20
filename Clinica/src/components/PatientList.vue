<template>
  <div class="patient-list">
    <FiltersBar :search="search" @search-change="handleSearchChange">
      <button class="btn btn-outline-primary" @click="$router.push('/pacientes/nuevo')">
        + Nuevo Paciente
      </button>
    </FiltersBar>

    <div v-if="loading" class="loading">Cargando pacientes...</div>
    <div v-else-if="error" class="error-text">{{ error }}</div>
    <div v-else-if="patients.length === 0" class="empty">
      No se encontraron pacientes
    </div>
    <div v-else>
      <table class="table">
        <thead>
          <tr>
            <th>Nombre Completo</th>
            <th>Documento</th>
            <th>Teléfono</th>
            <th>Email</th>
            <th>Seguro</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="patient in patients" :key="patient.id">
            <td>{{ patient.firstName }} {{ patient.lastName }}</td>
            <td>{{ patient.document }}</td>
            <td>{{ patient.phone || '-' }}</td>
            <td>{{ patient.email || '-' }}</td>
            <td>{{ patient.insuranceName }}</td>
            <td class="actions-cell">
              <button 
                class="btn btn-outline-primary btn-sm"
                @click="$router.push(`/pacientes/editar/${patient.id}`)"
              >
                Editar
              </button>
              <button 
                class="btn btn-outline-danger" 
                @click="handleDelete(patient)"
              >
                Borrar
              </button>
            </td>
          </tr>
        </tbody>
      </table>

      />
    </div>

    <ConfirmationModal
      :show="showDeleteModal"
      title="Eliminar Paciente"
      :message="`¿Estás seguro de que deseas eliminar al paciente ${patientToDelete?.firstName} ${patientToDelete?.lastName}?`"
      confirmText="Eliminar"
      @confirm="executeDelete"
      @cancel="showDeleteModal = false"
    />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { fetchPatients, deletePatient } from '../api/patients';
import FiltersBar from './FiltersBar.vue';
import Paginator from './Paginator.vue';
import ConfirmationModal from './ConfirmationModal.vue';

const router = useRouter();
const patients = ref([]);
const loading = ref(false);
const error = ref('');
const search = ref('');
const currentPage = ref(1);
const pageSize = 10;
const totalPages = ref(1);
const totalItems = ref(0);

const showDeleteModal = ref(false);
const patientToDelete = ref(null);

function handleDelete(patient) {
  patientToDelete.value = patient;
  showDeleteModal.value = true;
}

async function executeDelete() {
  if (!patientToDelete.value) return;
  
  try {
    await deletePatient(patientToDelete.value.id);
    showDeleteModal.value = false;
    patientToDelete.value = null;
    await loadPatients();
  } catch (err) {
    if (err.response?.status === 500) {
      alert('Api no disponible');
    } else {
      alert('Error al eliminar el paciente');
    }
    console.error(err);
  }
}
async function loadPatients() {
  loading.value = true;
  error.value = '';
  try {
    const result = await fetchPatients(currentPage.value, pageSize, search.value || null);
    console.log('API Response:', result);
    
    if (Array.isArray(result)) {
      patients.value = result;
      totalItems.value = result.length;
      totalPages.value = 1;
    } else {
      patients.value = result.items || [];
      currentPage.value = result.currentPage || 1;
      totalPages.value = result.totalPages || 1;
      totalItems.value = result.totalItems || 0;
    }
  } catch (err) {
    if (err.response?.status === 401) {
      error.value = 'Sesión no autorizada. Redirigiendo...';
    } else if (err.response?.status === 500) {
      error.value = 'Api no disponible';
    } else {
      const errorMessage = err.response?.data?.message || err.message || 'Error al cargar pacientes';
      error.value = `Error: ${errorMessage}`;
    }
    console.error('Error loading patients:', err);
    patients.value = [];
  } finally {
    loading.value = false;
  }
}

function handleSearchChange(value) {
  search.value = value;
  currentPage.value = 1;
  loadPatients();
}

function handlePageChange(page) {
  currentPage.value = page;
  loadPatients();
}


onMounted(loadPatients);
</script>

<style scoped>
.patient-list {
  width: 100%;
}
.loading,
.empty,
.error-text {
  padding: 40px;
  text-align: center;
  font-size: 16px;
}
</style>
