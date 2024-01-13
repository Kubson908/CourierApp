<script setup lang="ts">
import { onMounted } from "vue";
import { useRoute } from "vue-router";
import axios from "axios";
import { prefix } from "../../config";
import { loading, router } from "../../main";

const token = useRoute().query.token?.toString()!;

onMounted(async () => {
  try {
    const request = axios.create({
      baseURL: `${prefix}/api`,
      timeout: 10000,
    });
    request.interceptors.request.use((config) => {
      config.headers.Authorization = token.toString().replace("\xa0", "+");
      return config;
    });
    const res = await request.patch("/auth/confirm-email");
    if (res.status < 300) loading.value = false;
  } catch {}
});
</script>

<template>
  <div class="card">
    <div v-if="loading">
      <img src="/src/assets/loading.gif" height="200" />
    </div>
    <div v-else class="pigment-green-text">
      <h2>Adres e-mail został zweryfikowany</h2>
      <button class="submit" @click="router.push('/login')">Zaloguj się</button>
    </div>
  </div>
</template>

<style scoped>
.card {
  background-color: white;
  width: 30vw;
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  border-radius: 12px;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.25);
}
</style>
../../structures/config
