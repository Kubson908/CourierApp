<script setup lang="ts">
import { onBeforeMount, ref } from "vue";
import { router, unauthorized, user } from "../main";

const login = ref("");
const password = ref("");
const remember_me = ref(false);

onBeforeMount(() => {
  if (user.isLoggedIn) router.push("/");
});

const signIn = async () => {
  try {
    const res = await unauthorized.post("/auth/login", {
      login: login.value,
      password: password.value,
    });
    await localStorage.setItem("token", res.data.accessToken);
    if (remember_me.value)
      localStorage.setItem("expireDate", res.data.expireDate);
    else {
      let time = new Date(Date.now());
      time.setTime(time.getTime() + 60 * 60 * 1000);
      localStorage.setItem("expireDate", time.toString());
    }
    localStorage.setItem("user", res.data.user);
    localStorage.setItem("roles", JSON.stringify(res.data.roles));
    user.name = res.data.user;
    user.isLoggedIn = true;
    user.roles = res.data.roles;
    if (user.roles.includes("Admin")) {
      router.push("/administration");
    } else router.push("/");
  } catch (error: any) {
    console.log(error);
  }
};
</script>

<template>
  <div>
    <form @submit.prevent="signIn">
      <input v-model="login" placeholder="Login" type="text" />
      <input v-model="password" placeholder="Hasło" type="password" />
      <input id="rememberMe" v-model="remember_me" type="checkbox" />
      <label for="rememberMe">Zapamiętaj mnie</label>
      <button type="submit" @click="signIn">Zaloguj</button>
    </form>
  </div>
</template>

<style scoped></style>
