<template>
  <div class="service-list">
    <FiltersBar :search="search" @search-change="handleSearchChange">
      <button class="btn btn-outline-primary" @click="$router.push('/servicios/nuevo')">
        + Nuevo Servicio
      </button>
    </FiltersBar>

    <div v-if="loading" class="loading">Cargando servicios...</div>
    <div v-else-if="error" class="error-text">{{ error }}</div>
    <div v-else-if="services.length === 0" class="empty">
      No se encontraron servicios
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
          <tr v-for="service in services" :key="service.id">
            <td>{{ service.name }}</td>
            <td>{{ service.description || '-' }}</td>
            <td class="actions-cell">
              <button 
                class="btn btn-outline-primary btn-sm"
                @click="$router.push(`/servicios/editar/${service.id}`)"
              >
                Editar
              </button>
              <button 
                class="btn btn-outline-danger" 
                @click="handleDelete(service)"
              >
                Borrar
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <ConfirmationModal
      :show="showDeleteModal"
      title="Eliminar Servicio"
      :message="`¿Estás seguro de que deseas eliminar el servicio ${serviceToDelete?.name}?`"
      confirmText="Eliminar"
      :confirmDisabled="serviceToDelete?.isAssignedToDoctors"
      disabledMessage="No se puede eliminar un servicio que está asignado a uno o más doctores"
      @confirm="executeDelete"
      @cancel="showDeleteModal = false"
    />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { fetchServices, deleteService } from '../api/services';
import FiltersBar from './FiltersBar.vue';
import ConfirmationModal from './ConfirmationModal.vue';

const router = useRouter();
const services = ref([]);
const loading = ref(false);
const error = ref('');
const search = ref('');

const showDeleteModal = ref(false);
const serviceToDelete = ref(null);

function handleDelete(service) {
  serviceToDelete.value = service;
  showDeleteModal.value = true;
}

async function executeDelete() {
  if (!serviceToDelete.value) return;
  try {
    await deleteService(serviceToDelete.value.id);
    showDeleteModal.value = false;
    serviceToDelete.value = null;
    await loadServices();
  } catch (err) {
    if (err.response?.status === 500) {
      alert('Api no disponible');
    } else {
      alert('Error al eliminar el servicio');
    }
    console.error(err);
  }
}

async function loadServices() {
  loading.value = true;
  error.value = '';
  try {
    const data = await fetchServices();
    // Assuming backend returns an array since we didn't implement pagination for services
    if (Array.isArray(data)) {
        services.value = data;
    } else {
        services.value = data.items || [];
    }
  } catch (err) {
    if (err.response?.status === 401) {
      error.value = 'Sesión no autorizada (401).';
    } else if (err.response?.status === 500) {
      error.value = 'Api no disponible';
    } else {
      const errorMessage = err.response?.data?.message || err.message || 'Error al cargar servicios';
      error.value = `Error: ${errorMessage}`;
    }
    console.error('Error loading services:', err);
    services.value = [];
  } finally {
    loading.value = false;
  }
}

function handleSearchChange(value) {
  search.value = value;
  // Local filtering if we don't want to hit the API for every keystroke without server-side search
  // but for consistency with others, we expect server-side if we had it.
  // Our backend doesn't have search for services yet, so we could do local search.
  loadServices(); 
}


onMounted(loadServices);
</script>

<style scoped>
* {
  color: black;
}

.service-list {
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
