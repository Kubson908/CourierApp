<script setup lang="ts">
const emit = defineEmits(["submit", "cancelSubmit"]);
</script>

<template>
  <div class="fog">
    <div class="modal">
      <h3 class="pigment-green-text">Czy zatwierdzić trasę?</h3>
      <div class="button-container">
        <button @click="emit('cancelSubmit')" class="submit spacing">
          Anuluj
        </button>
        <button @click="emit('submit')" class="pigment-green submit spacing">
          Zatwierdź
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.modal {
  width: 40vw;
  height: fit-content;
  padding: 20px 20px;
  z-index: 1;
  background: #ffffff;
  border-radius: 30px;
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  margin: auto;
}
.button-container {
  width: 60%;
  margin: auto;
  display: grid;
  grid-auto-flow: column;
  grid-column-gap: 0px;
}
.spacing {
  width: 90%;
}
</style>
