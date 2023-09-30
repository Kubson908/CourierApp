<script setup lang="ts">
const props = defineProps<{
  user: any;
}>();

const emit = defineEmits(["edit"]);
const edit = () => {
  emit("edit");
};
</script>

<template>
  <div class="admin-card">
    <h3>{{ props.user.firstName + " " + props.user.lastName }}</h3>
    <button @click="edit">Edytuj</button>
  </div>
</template>

<style scoped>
.admin-card {
  background-color: #1b1919;
  display: inline-block;
  margin: 0 0 1% 1%;
  min-width: 20%;
  padding: 0 20px;
}
</style>
