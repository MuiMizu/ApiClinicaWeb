import { createRouter, createWebHistory } from 'vue-router';
import PatientsList from '../views/PatientsList.vue';
import PatientForm from '../views/PatientForm.vue';
import AppointmentsList from '../views/AppointmentsList.vue';
import AppointmentForm from '../views/AppointmentForm.vue';

const routes = [
  { path: '/', redirect: '/pacientes' },
  { path: '/pacientes', component: PatientsList },
  { path: '/pacientes/nuevo', component: PatientForm },
  { path: '/pacientes/editar/:id', component: PatientForm, props: true },
  { path: '/citas', component: AppointmentsList },
  { path: '/citas/nueva', component: AppointmentForm },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;

