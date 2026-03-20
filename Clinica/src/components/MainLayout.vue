<template>
  <div class="dashboard-container">
    <!-- Sidebar -->
    <aside class="sidebar" :class="{ 'collapsed': isCollapsed }">
      <div class="logo-section">
        <div class="logo-cube">C</div>
        <span class="logo-text" v-if="!isCollapsed">linica</span>
      </div>

      <nav class="sidebar-nav">
        <router-link to="/pacientes" class="nav-item" active-class="active">
          <div class="icon patients-icon"></div>
          <span v-if="!isCollapsed">Pacientes</span>
        </router-link>

        <router-link to="/citas" class="nav-item" active-class="active">
          <div class="icon appointments-icon"></div>
          <span v-if="!isCollapsed">Citas</span>
        </router-link>

        <router-link to="/doctores" class="nav-item" active-class="active">
          <div class="icon doctors-icon"></div>
          <span v-if="!isCollapsed">Doctores</span>
        </router-link>

        <router-link to="/servicios" class="nav-item" active-class="active">
          <div class="icon services-icon"></div>
          <span v-if="!isCollapsed">Servicios</span>
        </router-link>

        <router-link to="/seguros" class="nav-item" active-class="active">
          <div class="icon insurance-icon"></div>
          <span v-if="!isCollapsed">Seguros</span>
        </router-link>
      </nav>

      <div class="sidebar-footer">
        <button class="logout-btn" @click="handleLogout">
          <span class="icon logout-icon"></span>
          <span v-if="!isCollapsed">Cerrar Sesión</span>
        </button>
        
        <button class="collapse-btn" @click="isCollapsed = !isCollapsed">
          <span class="icon collapse-icon"></span>
          <span v-if="!isCollapsed">Colapsar menú</span>
        </button>
      </div>
    </aside>

    <div class="main-wrapper">
      <main class="page-content">
        <slot></slot>
      </main>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { logout } from '../stores/auth';

const router = useRouter();
const isCollapsed = ref(false);

const handleLogout = () => {
  logout();
  router.push('/login');
};
</script>

<style scoped>
.dashboard-container {
  display: flex;
  min-height: 100vh;
  background-color: #f8f9fa;
  color: #1a1a1a;
  font-family: 'Inter', sans-serif;
}

.sidebar {
  width: 260px;
  background: white;
  border-right: 1px solid #edf2f7;
  display: flex;
  flex-direction: column;
  transition: width 0.3s ease;
  z-index: 100;
  position: sticky;
  top: 0;
  height: 100vh;
}

.sidebar.collapsed { width: 80px; }
.logo-section { padding: 32px 24px; display: flex; align-items: center; gap: 4px; }

.logo-cube { 
  min-width: 40px; 
  height: 40px; 
  background: #5d5ae5; 
  border-radius: 10px; 
  display: flex; 
  align-items: center; 
  justify-content: center; 
  color: white; 
  font-weight: 900; 
  font-size: 24px;
}

.logo-text { font-size: 24px; font-weight: 800; color: #1a1a1a; margin-left: 2px; }
.sidebar-nav { flex: 1; padding: 10px 16px; overflow-y: auto; }

.nav-item {
  display: flex;
  align-items: center;
  padding: 14px 16px;
  margin-bottom: 8px;
  border-radius: 12px;
  color: #64748b;
  text-decoration: none;
  font-weight: 600;
  transition: all 0.2s;
  cursor: pointer;
}

.nav-item:hover { background: #f1f5f9; color: #1a1a1a; }
.nav-item.active { background: #5d5ae5; color: white !important; }

.nav-item .icon {
  width: 20px; height: 20px;
  margin-right: 12px;
  background-color: currentColor;
  mask-size: contain; mask-repeat: no-repeat; mask-position: center;
}

.collapsed .nav-item .icon { margin-right: 0; }
.collapsed .logo-section { justify-content: center; padding: 32px 0; }

.patients-icon { mask-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' fill='none' viewBox='0 0 24 24' stroke='currentColor'%3E%3Cpath stroke-linecap='round' stroke-linejoin='round' stroke-width='2' d='M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z' /%3E%3C/svg%3E"); }
.appointments-icon { mask-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' fill='none' viewBox='0 0 24 24' stroke='currentColor'%3E%3Cpath stroke-linecap='round' stroke-linejoin='round' stroke-width='2' d='M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z' /%3E%3C/svg%3E"); }
.doctors-icon { mask-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' fill='none' viewBox='0 0 24 24' stroke='currentColor'%3E%3Cpath stroke-linecap='round' stroke-linejoin='round' stroke-width='2' d='M21 13.255A23.931 23.931 0 0112 15c-3.183 0-6.22-.62-9-1.745M16 6V4a2 2 0 00-2-2h-4a2 2 0 00-2 2v2m4 6h.01M5 20h14a2 2 0 002-2V8a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z' /%3E%3C/svg%3E"); }
.services-icon { mask-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' fill='none' viewBox='0 0 24 24' stroke='currentColor'%3E%3Cpath stroke-linecap='round' stroke-linejoin='round' stroke-width='2' d='M19.428 15.428a2 2 0 00-1.022-.547l-2.387-.477a6 6 0 00-3.86.517l-.318.158a6 6 0 01-3.86.517L6.05 15.21a2 2 0 00-1.806.547M8 4h8l-1 1v5.172a2 2 0 00.586 1.414l5 5c1.26 1.26.367 3.414-1.415 3.414H4.828c-1.782 0-2.674-2.154-1.414-3.414l5-5A2 2 0 009 10.172V5L8 4z' /%3E%3C/svg%3E"); }
.insurance-icon { mask-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' fill='none' viewBox='0 0 24 24' stroke='currentColor'%3E%3Cpath stroke-linecap='round' stroke-linejoin='round' stroke-width='2' d='M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z' /%3E%3C/svg%3E"); }

.sidebar-footer { padding: 16px; margin-top: auto; }
.logout-btn {
  width: 100%;
  display: flex;
  align-items: center;
  padding: 12px 16px;
  background: transparent;
  border: 1px solid #fee2e2;
  border-radius: 12px;
  color: #dc2626;
  cursor: pointer;
  gap: 12px;
  margin-bottom: 12px;
  font-weight: 600;
  transition: all 0.2s;
}
.logout-btn:hover { background: #fef2f2; }
.logout-icon { 
  min-width: 18px; 
  min-height: 18px; 
  background-color: currentColor; 
  mask-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' fill='none' viewBox='0 0 24 24' stroke='currentColor'%3E%3Cpath stroke-linecap='round' stroke-linejoin='round' stroke-width='2' d='M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1' /%3E%3C/svg%3E"); mask-size: contain; mask-repeat: no-repeat;
}
.collapsed .logout-btn { justify-content: center; border: none; padding: 12px 0; }

.collapse-btn { width: 100%; display: flex; align-items: center; padding: 12px; background: transparent; border: none; color: #64748b; cursor: pointer; gap: 12px; }
.collapse-icon { min-width: 20px; min-height: 20px; background-color: currentColor; mask-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' fill='none' viewBox='0 0 24 24' stroke='currentColor'%3E%3Cpath stroke-linecap='round' stroke-linejoin='round' stroke-width='2' d='M11 19l-7-7 7-7m8 14l-7-7 7-7' /%3E%3C/svg%3E"); mask-size: contain; mask-repeat: no-repeat; }
.collapsed .collapse-btn { justify-content: center; }

.main-wrapper { flex: 1; display: flex; flex-direction: column; overflow-x: hidden; }
.page-content { flex: 1; padding: 24px; }
</style>
