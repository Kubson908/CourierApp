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
</script>

<template width="100%">
  <NavBar />
  <SideBar id="sidebar" v-if="user.roles.includes('Admin')" />
  <LoadingPage v-if="loading" id="loading" />
  <div :class="{ content: user.roles.includes('Admin') }">
    <router-view v-slot="{ Component }">
      <component class="mt-12" :is="Component" />
    </router-view>
  </div>
</template>

<style scoped>
.logo {
  height: 6em;
  padding: 1.5em;
  will-change: filter;
  transition: filter 300ms;
}
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
</style>
