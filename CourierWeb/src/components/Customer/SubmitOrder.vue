<script setup lang="ts">
const emit = defineEmits(["cancel", "submit"]);
const props = defineProps<{
  xs: number;
  s: any;
  m: number;
  l: number;
}>();
</script>

<template>
  <div class="card">
    <h2 class="pigment-green-text">Podsumowanie zamówienia</h2>
    <div class="flex-col black-text">
      <div v-if="props.xs > 0">Przesyłki bardzo małe: {{ props.xs }}</div>
      <div v-if="props.s > 0">Przesyłki małe: {{ props.xs }}</div>
      <div v-if="props.m > 0">Przesyłki średnie: {{ props.xs }}</div>
      <div v-if="props.l > 0">Przesyłki duże: {{ props.xs }}</div>
    </div>
    <button class="submit" @click="emit('cancel')">Wróć do edycji</button>
    <button class="submit pigment-green" @click="emit('submit')">
      Potwierdź zamówienie
    </button>
  </div>
</template>

<style scoped>
.card {
  background-color: white;
  width: 30vw;
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  border-radius: 12px;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.25);
  flex-direction: column;
  display: flex;
}
</style>
