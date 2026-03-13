import http from './http';

export async function fetchServices() {
  const { data } = await http.get('/services');
  return data;
}

export async function fetchServiceById(id) {
  const { data } = await http.get(`/services/${id}`);
  return data;
}

export async function createService(service) {
  const { data } = await http.post('/services', service);
  return data;
}

export async function updateService(id, service) {
  const { data } = await http.put(`/services/${id}`, service);
  return data;
}

export async function deleteService(id) {
  const { data } = await http.delete(`/services/${id}`);
  return data;
}
