import { createRouter, createWebHistory } from 'vue-router';
import PatientsList from '../views/PatientsList.vue';
import PatientForm from '../views/PatientForm.vue';
import AppointmentsList from '../views/AppointmentsList.vue';
import AppointmentForm from '../views/AppointmentForm.vue';

import LoginView from '../views/LoginView.vue';

const routes = [
  { path: '/', redirect: '/pacientes' },
  { path: '/login', component: LoginView },
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

router.beforeEach((to, from, next) => {
  const publicPages = ['/login'];
  const authRequired = !publicPages.includes(to.path);
  const loggedIn = localStorage.getItem('token');

  if (authRequired && !loggedIn) {
    return next('/login');
  }
  next();
});

export default router;

