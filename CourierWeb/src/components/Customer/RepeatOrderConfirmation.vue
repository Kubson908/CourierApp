<script setup lang="ts">
const emit = defineEmits(["cancel", "confirm"]);
</script>

<template>
  <div class="fog">
    <div class="modal">
      <h2 class="pigment-green-text">Czy ponowić zamówienie?</h2>
      <div style="margin-bottom: 20px;">
        <button class="submit black space" @click="emit('cancel')">Anuluj</button>
        <button class="submit pigment-green space" @click="emit('confirm')">
          Ponów
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.modal {
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
  max-height: 75vh;
  overflow-y: auto;
}
.space {
  margin-left: 10px;
  margin-right: 10px;
}
</style>
