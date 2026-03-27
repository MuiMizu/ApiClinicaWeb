<template>
  <transition name="fade">
    <div v-if="show" class="modal-overlay" @click.self="cancel">
      <div class="modal-container glassmorphism">
        <div class="modal-icon" :class="type">
          <i v-if="type === 'danger'" class="fas fa-exclamation-triangle"></i>
          <i v-else-if="type === 'warning'" class="fas fa-exclamation-circle"></i>
          <i v-else class="fas fa-question-circle"></i>
        </div>
        <h2 class="modal-title">{{ title }}</h2>
        <p class="modal-message">{{ message }}</p>
        
        <div v-if="confirmDisabled" class="alert alert-warning mb-4">
          <i class="fas fa-info-circle me-2"></i>
          {{ disabledMessage }}
        </div>

        <div class="modal-actions">
          <button class="btn btn-secondary" @click="cancel">
            {{ confirmDisabled ? 'Cerrar' : 'Cancelar' }}
          </button>
          <button 
            v-if="!confirmDisabled"
            class="btn" 
            :class="confirmBtnClass" 
            @click="confirm"
          >
            {{ confirmText }}
          </button>
        </div>
      </div>
    </div>
  </transition>
</template>

<script setup>
import { computed } from 'vue';

const props = defineProps({
  show: Boolean,
  title: { type: String, default: '¿Estás seguro?' },
  message: { type: String, default: 'Esta acción no se puede deshacer.' },
  confirmText: { type: String, default: 'Confirmar' },
  type: { type: String, default: 'danger' }, // danger, warning, info
  confirmDisabled: { type: Boolean, default: false },
  disabledMessage: { type: String, default: '' },
});

const emit = defineEmits(['confirm', 'cancel']);

const confirmBtnClass = computed(() => {
  return {
    'btn-danger': props.type === 'danger',
    'btn-warning': props.type === 'warning',
    'btn-primary': props.type === 'info',
  };
});

function confirm() {
  emit('confirm');
}

function cancel() {
  emit('cancel');
}
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  background: rgba(0, 0, 0, 0.4);
  backdrop-filter: blur(8px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 9999;
}

.modal-container {
  width: 90%;
  max-width: 400px;
  padding: 32px;
  background: rgba(255, 255, 255, 0.85);
  border-radius: 24px;
  border: 1px solid rgba(255, 255, 255, 0.3);
  box-shadow: 0 8px 32px 0 rgba(31, 38, 135, 0.2);
  text-align: center;
  animation: slideUp 0.3s ease-out;
}

.glassmorphism {
  background: rgba(255, 255, 255, 0.7);
  box-shadow: 0 8px 32px 0 rgba(31, 38, 135, 0.37);
  backdrop-filter: blur(12px);
  -webkit-backdrop-filter: blur(12px);
  border: 1px solid rgba(255, 255, 255, 0.18);
}

.modal-icon {
  width: 64px;
  height: 64px;
  margin: 0 auto 20px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 28px;
}

.modal-icon.danger {
  background: #fee2e2;
  color: #dc2626;
}

.modal-icon.warning {
  background: #fef3c7;
  color: #d97706;
}

.modal-icon.info {
  background: #e0f2fe;
  color: #0284c7;
}

.modal-title {
  font-size: 22px;
  font-weight: 700;
  color: #1e293b;
  margin-bottom: 12px;
}

.modal-message {
  font-size: 15px;
  color: #64748b;
  margin-bottom: 32px;
  line-height: 1.5;
}

.modal-actions {
  display: flex;
  gap: 12px;
  justify-content: center;
}

.btn {
  padding: 10px 24px;
  border-radius: 12px;
  font-weight: 600;
  font-size: 14px;
  cursor: pointer;
  transition: all 0.2s;
  border: none;
}

.btn-secondary {
  background: #f1f5f9;
  color: #475569;
}

.btn-secondary:hover {
  background: #e2e8f0;
}

.btn-danger {
  background: #dc2626;
  color: white;
}

.btn-danger:hover {
  background: #b91c1c;
}

.btn-warning {
  background: #d97706;
  color: white;
}

.btn-primary {
  background: #3b82f6;
  color: white;
}

.alert {
  padding: 12px;
  border-radius: 12px;
  font-size: 14px;
  margin-bottom: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.alert-warning {
  background: #fffbeb;
  color: #92400e;
  border: 1px solid #fde68a;
}

.mb-4 { margin-bottom: 1.5rem; }
.me-2 { margin-right: 0.5rem; }

/* Animations */
@keyframes slideUp {
  from {
    transform: translateY(20px);
    opacity: 0;
  }
  to {
    transform: translateY(0);
    opacity: 1;
  }
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
