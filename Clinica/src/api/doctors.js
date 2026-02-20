import http from './http';

export async function fetchDoctors(page = 1, pageSize = 10, search = null) {
  const params = new URLSearchParams({ page: page.toString(), pageSize: pageSize.toString() });
  if (search) params.append('search', search);
  const { data } = await http.get(`/doctors?${params}`);
  return data;
}

export async function fetchDoctorById(id) {
  const { data } = await http.get(`/doctors/${id}`);
  return data;
}

export async function createDoctor(payload) {
  const { data } = await http.post('/doctors', payload);
  return data;
}

export async function updateDoctor(id, payload) {
  const { data } = await http.put(`/doctors/${id}`, payload);
  return data;
}

export async function deleteDoctor(id) {
  await http.delete(`/doctors/${id}`);
}

export async function fetchAvailableDoctors(serviceId, date) {
  const dateStr = typeof date === 'string' ? date : new Date(date).toISOString().split('T')[0];
  const { data } = await http.get(`/doctors/available?serviceId=${serviceId}&date=${dateStr}`);
  return data;
}

