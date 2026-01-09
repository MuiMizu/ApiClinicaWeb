import http from './http';

export async function fetchPatients(page = 1, pageSize = 10, search = null) {
  const params = new URLSearchParams({ page: page.toString(), pageSize: pageSize.toString() });
  if (search) params.append('search', search);
  const { data } = await http.get(`/patients?${params}`);
  return data;
}

export async function fetchPatientById(id) {
  const { data } = await http.get(`/patients/${id}`);
  return data;
}

export async function createPatient(payload) {
  const { data } = await http.post('/patients', payload);
  return data;
}

export async function updatePatient(id, payload) {
  const { data } = await http.put(`/patients/${id}`, payload);
  return data;
}

export async function deletePatient(id) {
  await http.delete(`/patients/${id}`);
}

