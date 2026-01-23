<template>
  <div class="paginator">
    <button 
      class="btn ghost" 
      @click="$emit('page-change', currentPage - 1)"
      :disabled="currentPage === 1"
    >
      Anterior
    </button>
    <span class="page-info">
      Página {{ currentPage }} de {{ totalPages }} ({{ totalItems }} total)
    </span>
    <button 
      class="btn ghost" 
      @click="$emit('page-change', currentPage + 1)"
      :disabled="currentPage >= totalPages"
    >
      Siguiente
    </button>
  </div>
</template>

<script setup>
defineProps({
  currentPage: { type: Number, required: true },
  totalPages: { type: Number, required: true },
  totalItems: { type: Number, required: true },
});

defineEmits(['page-change']);
</script>

<style scoped>
*{
  color: black;
}

.paginator {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 16px;
  margin-top: 24px;
  padding: 16px;
}

.page-info {
  font-size: 14px;
  color: #666;
  min-width: 200px;
  text-align: center;
}
</style>

