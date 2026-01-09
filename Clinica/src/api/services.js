import http from './http';

export async function fetchServices() {
  const { data } = await http.get('/services');
  return data;
}

