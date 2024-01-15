<script setup lang="ts">
import { onBeforeMount } from "vue";
import { NavBar, SideBar } from "./components";
import { user, authorized } from "./main";

onBeforeMount(async () => {
  if (!localStorage.getItem("expireDate")) return;
  const date = Date.parse(localStorage.getItem("expireDate") as string);
  if (date && date < Date.now()) {
    localStorage.clear();
    user.name = "Niezalogowany";
    user.isLoggedIn = false;
    user.roles = [];
  } else if (localStorage.getItem("rememberMe") == "true") {
    const res = await authorized.get("/auth/refresh-token");
    localStorage.setItem("token", res.data.accessToken);
    localStorage.setItem("expireDate", res.data.expireDate);
  }
});

const message = "Aby skorzystać z aplikacji uruchom ją na komputerze";
</script>

<template width="100%">
  <h1 class="black-text invalid-device">
    {{ message }}
  </h1>
  <NavBar id="navbar" />
  <SideBar
    id="sidebar"
    v-if="user.roles.includes('Admin') || user.roles.includes('Dispatcher')"
  />
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
#navbar {
  z-index: 1000;
  position: fixed;
}
#sidebar {
  z-index: 900;
  top: 5.5vh;
  position: fixed;
}
.content {
  margin-left: 3%;
}
.center {
  text-align: center;
  min-height: 94.5vh;
  margin-top: 5.5vh !important;
}
.invalid-device {
  display: none;
}
@media screen and (max-width: 600px) {
  #navbar,
  #sidebar,
  .center {
    display: none;
  }
  .invalid-device {
    display: block;
    text-align: center;
  }
}
</style>
