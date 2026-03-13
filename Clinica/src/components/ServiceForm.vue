<template>
  <form @submit.prevent="handleSubmit" class="service-form">
    <div class="field">
      <div class="form-floating">
        <input 
          id="name"
          v-model.trim="form.name" 
          type="text"
          class="form-control"
          placeholder="Nombre del Servicio"
          required
          :disabled="loading"
        />
        <label for="name">Nombre *</label>
      </div>
    </div>

    <div class="field">
      <div class="form-floating">
        <textarea 
          id="description"
          v-model.trim="form.description" 
          class="form-control"
          placeholder="Descripción"
          style="height: 100px"
          :disabled="loading"
        ></textarea>
        <label for="description">Descripción</label>
      </div>
    </div>

    <div class="actions">
      <button 
        class="btn btn-outline-primary" 
        type="submit" 
        :disabled="submitting || loading"
      >
        {{ submitting ? 'Guardando...' : isEdit ? 'Actualizar' : 'Crear' }}
      </button>
      <button 
        class="btn btn-outline-info" 
        type="button" 
        @click="resetForm" 
        :disabled="submitting || loading"
      >
        Limpiar
      </button>
      <span v-if="message" :class="messageClass">{{ message }}</span>
    </div>
  </form>
</template>

<script setup>
import { computed, onMounted, reactive, ref } from 'vue';
import { createService, updateService, fetchServiceById } from '../api/services';

const props = defineProps({
  serviceId: { type: Number, default: null },
});

const emit = defineEmits(['saved']);

const submitting = ref(false);
const loading = ref(false);
const message = ref('');

const form = reactive({
  name: '',
  description: '',
});

const isEdit = computed(() => !!props.serviceId);

const messageClass = computed(() => {
  return message.value.includes('Error') || message.value.includes('error') 
    ? 'error-text' 
    : 'success-text';
});

onMounted(async () => {
  if (props.serviceId) {
    await loadService();
  }
});

async function loadService() {
  loading.value = true;
  try {
    const service = await fetchServiceById(props.serviceId);
    form.name = service.name;
    form.description = service.description || '';
  } catch (err) {
    message.value = 'Error al cargar el servicio';
    console.error(err);
  } finally {
    loading.value = false;
  }
}

function resetForm() {
  if (isEdit.value) {
    loadService();
  } else {
    form.name = '';
    form.description = '';
  }
  message.value = '';
}

async function handleSubmit() {
  submitting.value = true;
  message.value = '';
  
  try {
    const payload = {
      name: form.name,
      description: form.description || null,
    };
    
    if (props.serviceId) {
      await updateService(props.serviceId, payload);
      message.value = 'Servicio actualizado exitosamente';
    } else {
      await createService(payload);
      message.value = 'Servicio creado exitosamente';
      resetForm();
    }
    
    setTimeout(() => {
      emit('saved');
    }, 1500);
  } catch (err) {
    message.value = err?.response?.data?.message || 'Error al guardar el servicio';
    console.error(err);
  } finally {
    submitting.value = false;
  }
}
</script>

<style scoped>
* {
  color: black;
}

.service-form {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.error-text {
  color: #dc2626;
  font-size: 14px;
}

.success-text {
  color: #059669;
  font-size: 14px;
}

.form-floating > .form-control {
    height: calc(3.5rem + 2px);
    line-height: 1.25;
}

.form-floating > textarea.form-control {
    height: 100px;
}
</style>
