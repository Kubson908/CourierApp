<script setup lang="ts">
import { onBeforeMount, ref } from "vue";
import { authorized } from "../main";

const dispatchers = ref<any>(null);
onBeforeMount(async () => {
  const getDispatchers = await authorized.get("/admin/get-dispatchers");
  dispatchers.value = getDispatchers.data.map((dispatcher: any) => {
    return {
      name: dispatcher.firstName + " " + dispatcher.lastName,
      username: dispatcher.userName,
      email: dispatcher.email,
      phoneNumber: dispatcher.phoneNumber,
    };
  });
});
</script>

<template>
  <div id="adminDiv">
    <h1 color="#ff0000">ADMIN PAGE</h1>
    <div v-for="dispatcher in dispatchers" class="admin-card">
      <h3>{{ dispatcher.name }}</h3>
    </div>
  </div>
</template>

<style scoped>
#adminDiv {
  padding: 0 10%;
}

.admin-card {
  background-color: #1b1919;
  display: inline-block;
  margin: 0 10px;
  min-width: 20%;
  padding: 0 20px;
}
</style>
