import http from './http';

export async function fetchInsurances() {
  const { data } = await http.get('/insurances');
  return data;
}

