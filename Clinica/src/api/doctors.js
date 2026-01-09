import http from './http';

export async function fetchAvailableDoctors(serviceId, date) {
  const dateStr = typeof date === 'string' ? date : new Date(date).toISOString().split('T')[0];
  const { data } = await http.get(`/doctors/available?serviceId=${serviceId}&date=${dateStr}`);
  return data;
}

