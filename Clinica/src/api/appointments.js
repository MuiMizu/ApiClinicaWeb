import http from './http';

export async function fetchAppointments(filters = {}) {
  const params = new URLSearchParams();
  if (filters.date) params.append('date', filters.date);
  if (filters.doctorId) params.append('doctorId', filters.doctorId);
  if (filters.patientId) params.append('patientId', filters.patientId);
  if (filters.status !== undefined && filters.status !== null) params.append('status', filters.status);
  
  const { data } = await http.get(`/appointments?${params}`);
  return data;
}

export async function fetchAppointmentById(id) {
  const { data } = await http.get(`/appointments/${id}`);
  return data;
}

export async function createAppointment(payload) {
  const { data } = await http.post('/appointments', payload);
  return data;
}

export async function updateAppointmentStatus(id, status) {
  await http.patch(`/appointments/${id}/status`, { status });
}

export async function fetchAvailableTimes(doctorId, date) {
  const dateStr = typeof date === 'string' ? date : new Date(date).toISOString().split('T')[0];
  const { data } = await http.get(`/appointments/availability?doctorId=${doctorId}&date=${dateStr}`);
  return data;
}

