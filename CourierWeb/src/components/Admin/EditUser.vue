<script setup lang="ts">
const props = defineProps<{
  user: any;
}>();

const close_info = defineEmits(["closeModal"]);
const close = () => {
  close_info("closeModal");
};
</script>

<template>
  <div class="fog" @click.self="close">
    <div class="modal">
      {{ props.user.firstName }}
    </div>
  </div>
</template>

<style scoped>
.fog {
  background-color: rgba(83, 83, 83, 0.789);
  position: fixed;
  z-index: 1;
  width: 100%;
  height: 100%;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
}
.modal {
  width: 40vw;
  height: fit-content;
  padding: 0 20px;
  z-index: 1;
  background: #1b1919;
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  margin: auto;
}
</style>
