<template>
  <div class="appointments-list">
    <FiltersBar :search="filters.search" @search-change="handleSearchChange">
      <input
        v-model="filters.date"
        type="date"
        class="filter-input"
        placeholder="Fecha"
        @change="loadAppointments"
      />
      <select 
        v-model.number="filters.status" 
        @change="loadAppointments" 
        class="filter-input"
      >
        <option :value="null">Todos los estados</option>
        <option :value="0">Programada</option>
        <option :value="1">Atendida</option>
        <option :value="2">Cancelada</option>
      </select>
      <button class="btn primary" @click="$router.push('/citas/nueva')">
        + Nueva Cita
      </button>
    </FiltersBar>

    <div v-if="loading" class="loading">Cargando citas...</div>
    <div v-else-if="error" class="error">{{ error }}</div>
    <div v-else-if="appointments.length === 0" class="empty">
      No se encontraron citas
    </div>
    <div v-else>
      <table class="table">
        <thead>
          <tr>
            <th>Fecha</th>
            <th>Hora</th>
            <th>Paciente</th>
            <th>Médico</th>
            <th>Servicio</th>
            <th>Estado</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="appointment in appointments" :key="appointment.id">
            <td>{{ formatDate(appointment.date) }}</td>
            <td>{{ appointment.time }}</td>
            <td>{{ appointment.patientName }}</td>
            <td>{{ appointment.doctorName }}</td>
            <td>{{ appointment.serviceName || '-' }}</td>
            <td>
              <span :class="statusClass(appointment.status)">
                {{ statusText(appointment.status) }}
              </span>
            </td>
            <td class="actions-cell">
              <button
                v-if="appointment.status === 0"
                class="btn-link"
                @click="updateStatus(appointment.id, 1)"
              >
                Marcar Atendida
              </button>
              <button
                v-if="appointment.status === 0"
                class="btn-link danger"
                @click="updateStatus(appointment.id, 2)"
              >
                Cancelar
              </button>
              <span v-else class="note">-</span>
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
import { fetchAppointments, updateAppointmentStatus } from '../api/appointments';
import FiltersBar from './FiltersBar.vue';

const router = useRouter();
const appointments = ref([]);
const loading = ref(false);
const error = ref('');

const filters = ref({
  search: '',
  date: '',
  status: null,
});

async function loadAppointments() {
  loading.value = true;
  error.value = '';
  try {
    const filterParams = {};
    if (filters.value.date) filterParams.date = filters.value.date;
    if (filters.value.status !== null) filterParams.status = filters.value.status;
    
    appointments.value = await fetchAppointments(filterParams);
  } catch (err) {
    error.value = 'Error al cargar citas';
    console.error(err);
  } finally {
    loading.value = false;
  }
}

function handleSearchChange(value) {
  filters.value.search = value;
  // La búsqueda se puede implementar si el backend lo soporta
  loadAppointments();
}

function formatDate(dateString) {
  if (!dateString) return '-';
  const date = new Date(dateString);
  return date.toLocaleDateString('es-ES', { 
    year: 'numeric', 
    month: '2-digit', 
    day: '2-digit' 
  });
}

function statusText(status) {
  const map = { 0: 'Programada', 1: 'Atendida', 2: 'Cancelada' };
  return map[status] || 'Desconocido';
}

function statusClass(status) {
  const map = {
    0: 'status-scheduled',
    1: 'status-completed',
    2: 'status-cancelled',
  };
  return map[status] || '';
}

async function updateStatus(id, status) {
  const action = status === 1 ? 'marcar como atendida' : 'cancelar';
  if (!confirm(`¿Estás seguro de ${action} esta cita?`)) return;
  
  try {
    await updateAppointmentStatus(id, status);
    await loadAppointments();
  } catch (err) {
    alert('Error al actualizar el estado de la cita');
    console.error(err);
  }
}

onMounted(loadAppointments);
</script>

<style scoped>
.appointments-list {
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

.status-scheduled {
  padding: 4px 12px;
  background: #fef3c7;
  color: #92400e;
  border-radius: 6px;
  font-size: 12px;
  font-weight: 500;
}

.status-completed {
  padding: 4px 12px;
  background: #d1fae5;
  color: #065f46;
  border-radius: 6px;
  font-size: 12px;
  font-weight: 500;
}

.status-cancelled {
  padding: 4px 12px;
  background: #fee2e2;
  color: #991b1b;
  border-radius: 6px;
  font-size: 12px;
  font-weight: 500;
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
.error,
.empty {
  padding: 40px;
  text-align: center;
  color: #6b7280;
  font-size: 16px;
}

.error {
  color: #dc2626;
}
</style>

