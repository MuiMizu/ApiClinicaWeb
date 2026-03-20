<template>
  <div class="insurance-list">
    <FiltersBar :search="search" @search-change="handleSearchChange">
      <button class="btn btn-outline-primary" @click="$router.push('/seguros/nuevo')">
        + Nuevo Seguro
      </button>
    </FiltersBar>

    <div v-if="loading" class="loading">Cargando seguros...</div>
    <div v-else-if="error" class="error-text">{{ error }}</div>
    <div v-else-if="insurances.length === 0" class="empty">
      No se encontraron seguros
    </div>
    <div v-else>
      <table class="table">
        <thead>
          <tr>
            <th>Nombre</th>
            <th>Descripción</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="insurance in insurances" :key="insurance.id">
            <td>{{ insurance.name }}</td>
            <td>{{ insurance.description || '-' }}</td>
            <td class="actions-cell">
              <button 
                class="btn btn-outline-primary btn-sm"
                @click="$router.push(`/seguros/editar/${insurance.id}`)"
              >
                Editar
              </button>
              <button 
                class="btn btn-outline-danger" 
                @click="handleDelete(insurance.id)"
              >
                Borrar
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { fetchInsurances, deleteInsurance } from '../api/insurances';
import FiltersBar from './FiltersBar.vue';

const router = useRouter();
const insurances = ref([]);
const loading = ref(false);
const error = ref('');
const search = ref('');

async function loadInsurances() {
  loading.value = true;
  error.value = '';
  try {
    const data = await fetchInsurances();
    if (Array.isArray(data)) {
        insurances.value = data;
    } else {
        insurances.value = data.items || [];
    }
  } catch (err) {
    if (err.response?.status === 401) {
      error.value = 'Sesión no autorizada (401).';
    } else if (err.response?.status === 500) {
      error.value = 'Api no disponible';
    } else {
      const errorMessage = err.response?.data?.message || err.message || 'Error al cargar seguros';
      error.value = `Error: ${errorMessage}`;
    }
    console.error('Error loading insurances:', err);
    insurances.value = [];
  } finally {
    loading.value = false;
  }
}

function handleSearchChange(value) {
  search.value = value;
  loadInsurances(); 
}

async function handleDelete(id) {
  if (!confirm('¿Estás seguro de eliminar este seguro?')) return;
  try {
    await deleteInsurance(id);
    await loadInsurances();
  } catch (err) {
    if (err.response?.status === 500) {
      alert('Api no disponible');
    } else {
      alert('Error al eliminar el seguro');
    }
    console.error(err);
  }
}

onMounted(loadInsurances);
</script>

<style scoped>
* {
  color: black;
}

.insurance-list {
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
