<script setup lang="ts">
import { onBeforeMount } from "vue";
import { NavBar, SideBar, LoadingPage } from "./components";
import { user, loading } from "./main";

onBeforeMount(() => {
  const date = Date.parse(localStorage.getItem("expireDate") as string);
  if (date < Date.now()) {
    localStorage.clear();
    user.name = "Niezalogowany";
    user.isLoggedIn = false;
    user.roles = [];
  }
});

const sidebar: boolean =
  user.roles.includes("Admin") || user.roles.includes("Dispatcher");
</script>

<template width="100%">
  <NavBar />
  <SideBar id="sidebar" v-if="sidebar" />
  <LoadingPage v-if="loading" id="loading" />
  <div
    :class="{
      content:
        user.roles.includes('Admin') || user.roles.includes('Dispatcher'),
      center: true,
    }"
  >
    <router-view v-slot="{ Component }">
      <component :is="Component" />
    </router-view>
  </div>
</template>

<style scoped>
#loading {
  z-index: 99;
  position: fixed;
}
.logo:hover {
  filter: drop-shadow(0 0 2em #646cffaa);
}
.logo.vue:hover {
  filter: drop-shadow(0 0 2em #42b883aa);
}
#sidebar {
  position: fixed;
}
.content {
  margin-left: 3%;
}

.center {
  text-align: center;
  min-height: 94.5vh;
}
</style>
