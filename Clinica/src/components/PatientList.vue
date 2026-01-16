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
                class="btn-link" 
                @click="$router.push(`/pacientes/editar/${patient.id}`)"
              >
                Editar
              </button>
              <button 
                class="btn-link danger" 
                @click="handleDelete(patient.id)"
              >
                Eliminar
              </button>
            </td>
          </tr>
        </tbody>
      </table>

      <Paginator
        v-if="totalPages > 1"
        :current-page="currentPage"
        :total-pages="totalPages"
        :total-items="totalItems"
        @page-change="handlePageChange"
      />
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { fetchPatients, deletePatient } from '../api/patients';
import FiltersBar from './FiltersBar.vue';
import Paginator from './Paginator.vue';

const router = useRouter();
const patients = ref([]);
const loading = ref(false);
const error = ref('');
const search = ref('');
const currentPage = ref(1);
const pageSize = 10;
const totalPages = ref(1);
const totalItems = ref(0);

async function loadPatients() {
  loading.value = true;
  error.value = '';
  try {
    const result = await fetchPatients(currentPage.value, pageSize, search.value || null);
    console.log('API Response:', result);
    patients.value = result.items || [];
    currentPage.value = result.currentPage || 1;
    totalPages.value = result.totalPages || 1;
    totalItems.value = result.totalItems || 0;
  } catch (err) {
    if (err.response?.status === 500) {
      error.value = 'Api no disponible';
    } else {
      const errorMessage = err.response?.data?.message || err.message || 'Error al cargar pacientes';
      error.value = `Error: ${errorMessage}`;
    }
    console.error('Error loading patients:', err);
    console.error('Error details:', err.response?.data);
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

async function handleDelete(id) {
  if (!confirm('¿Estás seguro de eliminar este paciente?')) return;
  try {
    await deletePatient(id);
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

onMounted(loadPatients);
</script>

<style scoped>
.patient-list {
  width: 100%;
}

.table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 16px;
  background: white;
  border-radius: 8px;
  overflow: hidden;
}

.table th,
.table td {
  padding: 12px 16px;
  text-align: left;
  border-bottom: 1px solid #e5e7eb;
}

.table th {
  background: #f9fafb;
  font-weight: 600;
  color: #374151;
  font-size: 14px;
}

.table tbody tr:hover {
  background: #f9fafb;
}

.actions-cell {
  display: flex;
  gap: 12px;
}

.btn-link {
  background: none;
  border: none;
  color: #2563eb;
  cursor: pointer;
  text-decoration: underline;
  font-size: 14px;
  padding: 0;
}

.btn-link:hover {
  color: #1d4ed8;
}

.btn-link.danger {
  color: #dc2626;
}

.btn-link.danger:hover {
  color: #b91c1c;
}

.loading,
.empty {
  padding: 40px;
  text-align: center;
  color: #6b7280;
  font-size: 16px;
}

.error-text {
  padding: 40px;
  text-align: center;
  color: #dc2626;
  font-size: 16px;
}
</style>

