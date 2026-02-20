<template>
  <div class="doctor-list">
    <FiltersBar :search="search" @search-change="handleSearchChange">
      <button class="btn btn-outline-primary" @click="$router.push('/doctores/nuevo')">
        + Nuevo Doctor
      </button>
    </FiltersBar>

    <div v-if="loading" class="loading">Cargando doctores...</div>
    <div v-else-if="error" class="error-text">{{ error }}</div>
    <div v-else-if="doctors.length === 0" class="empty">
      No se encontraron doctores
    </div>
    <div v-else>
      <table class="table">
        <thead>
          <tr>
            <th>Nombre Completo</th>
            <th>Especialidad</th>
            <th>Licencia</th>
            <th>Teléfono</th>
            <th>Email</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="doctor in doctors" :key="doctor.id">
            <td>{{ doctor.firstName }} {{ doctor.lastName }}</td>
            <td>{{ doctor.specialty || '-' }}</td>
            <td>{{ doctor.licenseNumber || '-' }}</td>
            <td>{{ doctor.phone || '-' }}</td>
            <td>{{ doctor.email || '-' }}</td>
            <td class="actions-cell">
              <button 
                class="btn btn-outline-primary btn-sm"
                @click="$router.push(`/doctores/editar/${doctor.id}`)"
              >
                Editar
              </button>
              <button 
                class="btn btn-outline-danger" 
                @click="handleDelete(doctor.id)"
              >
                Borrar
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
import { fetchDoctors, deleteDoctor } from '../api/doctors';
import FiltersBar from './FiltersBar.vue';
import Paginator from './Paginator.vue';

const router = useRouter();
const doctors = ref([]);
const loading = ref(false);
const error = ref('');
const search = ref('');
const currentPage = ref(1);
const pageSize = 10;
const totalPages = ref(1);
const totalItems = ref(0);

async function loadDoctors() {
  loading.value = true;
  error.value = '';
  try {
    const result = await fetchDoctors(currentPage.value, pageSize, search.value || null);
    console.log('Doctors API Response:', result);
    
    if (Array.isArray(result)) {
      doctors.value = result;
      totalItems.value = result.length;
      totalPages.value = 1;
    } else {
      doctors.value = result.items || [];
      currentPage.value = result.currentPage || 1;
      totalPages.value = result.totalPages || 1;
      totalItems.value = result.totalItems || 0;
    }
  } catch (err) {
    if (err.response?.status === 401) {
      error.value = 'Sesión no autorizada (401).';
    } else if (err.response?.status === 500) {
      error.value = 'Api no disponible';
    } else {
      const errorMessage = err.response?.data?.message || err.message || 'Error al cargar doctores';
      error.value = `Error: ${errorMessage}`;
    }
    console.error('Error loading doctors:', err);
    doctors.value = [];
  } finally {
    loading.value = false;
  }
}

function handleSearchChange(value) {
  search.value = value;
  currentPage.value = 1;
  loadDoctors();
}

function handlePageChange(page) {
  currentPage.value = page;
  loadDoctors();
}

async function handleDelete(id) {
  if (!confirm('¿Estás seguro de eliminar este doctor?')) return;
  try {
    await deleteDoctor(id);
    await loadDoctors();
  } catch (err) {
    if (err.response?.status === 500) {
      alert('Api no disponible');
    } else {
      alert('Error al eliminar el doctor');
    }
    console.error(err);
  }
}

onMounted(loadDoctors);
</script>

<style scoped>
* {
  color: black;
}

.doctor-list {
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
