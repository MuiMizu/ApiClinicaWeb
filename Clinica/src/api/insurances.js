import http from './http';

export async function fetchInsurances() {
  const { data } = await http.get('/insurances');
  return data;
}

export async function fetchInsuranceById(id) {
  const { data } = await http.get(`/insurances/${id}`);
  return data;
}

export async function createInsurance(insurance) {
  const { data } = await http.post('/insurances', insurance);
  return data;
}

export async function updateInsurance(id, insurance) {
  const { data } = await http.put(`/insurances/${id}`, insurance);
  return data;
}

export async function deleteInsurance(id) {
  const { data } = await http.delete(`/insurances/${id}`);
  return data;
}
