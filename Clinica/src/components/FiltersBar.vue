<template>
  <div class="filters-bar">
    <input
      v-model="localSearch"
      type="text"
      placeholder="Buscar..."
      class="filter-input"
      @input="handleSearch"
    />
    <slot />
  </div>
</template>

<script setup>
import { ref, watch } from 'vue';

const props = defineProps({
  search: { type: String, default: '' },
});

const emit = defineEmits(['search-change']);

const localSearch = ref(props.search);

watch(() => props.search, (newVal) => {
  localSearch.value = newVal;
});

function handleSearch() {
  emit('search-change', localSearch.value);
}
</script>

<style scoped>
.filters-bar {
  display: flex;
  gap: 12px;
  margin-bottom: 20px;
  flex-wrap: wrap;
  align-items: center;
}

.filter-input {
  flex: 1;
  min-width: 200px;
  padding: 10px 12px;
  border: 1px solid #cbd5e1;
  border-radius: 8px;
  font-size: 14px;
}

.filter-input:focus {
  outline: none;
  border-color: #2563eb;
  box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.1);
}
</style>

