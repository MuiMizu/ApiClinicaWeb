<template>
  <div class="availability-item" :class="availabilityClass">
    <span class="time">{{ time }}</span>
    <span class="slots">
      {{ slots }} {{ slots === 1 ? 'cupo' : 'cupos' }}
    </span>
  </div>
</template>

<script setup>
import { computed } from 'vue';

const props = defineProps({
  time: { type: String, required: true },
  slots: { type: Number, required: true },
});

const availabilityClass = computed(() => {
  if (props.slots === 0) return 'unavailable';
  if (props.slots === 1) return 'low';
  return 'available';
});
</script>

<style scoped>
.availability-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px 12px;
  border-radius: 6px;
  margin-bottom: 4px;
}

.time {
  font-weight: 600;
  font-size: 14px;
}

.slots {
  font-size: 12px;
  padding: 4px 8px;
  border-radius: 4px;
  font-weight: 500;
}

.available {
  background: #d1fae5;
}

.available .slots {
  background: #10b981;
  color: white;
}

.low {
  background: #fef3c7;
}

.low .slots {
  background: #f59e0b;
  color: white;
}

.unavailable {
  background: #fee2e2;
  opacity: 0.6;
}

.unavailable .slots {
  background: #ef4444;
  color: white;
}
</style>

