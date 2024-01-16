<script setup lang="ts">
import { onBeforeMount, ref } from "vue";
import { Shipment } from "../../typings/shipment";

const emit = defineEmits(["cancel", "submit"]);
const props = defineProps<{
  shipments: Array<Shipment>;
  price: string;
}>();

const xs = ref<number>(0);
const s = ref<number>(0);
const m = ref<number>(0);
const l = ref<number>(0);

onBeforeMount(() => {
  xs.value = props.shipments.filter((s) => s.size == 0).length;
  s.value = props.shipments.filter((s) => s.size == 1).length;
  m.value = props.shipments.filter((s) => s.size == 2).length;
  l.value = props.shipments.filter((s) => s.size == 3).length;
});
const loading = ref<boolean>(false);
</script>

<template>
  <div class="card">
    <div v-if="!loading">
      <h2 class="pigment-green-text">Podsumowanie zamówienia</h2>
      <div class="flex-col black-text">
        <div v-if="xs > 0">Przesyłki bardzo małe: {{ xs }}</div>
        <div v-if="s > 0">Przesyłki małe: {{ s }}</div>
        <div v-if="m > 0">Przesyłki średnie: {{ m }}</div>
        <div v-if="l > 0">Przesyłki duże: {{ l }}</div>
        <div>Kwota zamówienia: {{ props.price }}</div>
      </div>
      <div>
        <button class="black space" @click="emit('cancel')">
          Wróć do edycji
        </button>
        <button
          class="submit pigment-green space"
          @click="
            emit('submit');
            loading = true;
          "
        >
          Potwierdź zamówienie
        </button>
      </div>
    </div>
    <div v-else class="h-150">
      <img src="/src/assets/loading.gif" class="loading-icon" />
    </div>
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
.h-150 {
  height: 150px;
}
.loading-icon {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  height: 150px;
}
.space {
  margin-left: 10px;
  margin-right: 10px;
}
</style>
