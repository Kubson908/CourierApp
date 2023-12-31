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
    <div class="info">
      <h3>{{ props.user.firstName + " " + props.user.lastName }}</h3>
    </div>
    <button @click="edit" class="submit">Edytuj</button>
  </div>
</template>

<style scoped>
.admin-card {
  background-color: white;
  color: black;
  border-radius: 12px;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.25);
  min-width: 20%;
  padding: 0 20px;
}
.info {
  height: 25%;
}
</style>
