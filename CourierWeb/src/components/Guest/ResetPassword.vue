<script setup lang="ts">
import { ref } from "vue";
import axios from "axios";
import { prefix } from "../../config";
import { useRoute } from "vue-router";

const token = useRoute().query.token?.toString()!;

const password = ref<string>("");
const confirmPassword = ref<string>("");

const loading = ref<boolean>(false);
const errorMessage = ref<string>("");
const passwordReset = ref<boolean>(false);
// TODO: dodac rules do hasla
const resetPasword = async () => {
  if (password.value == "" || confirmPassword.value == "") {
    errorMessage.value = "Podaj nowe hasło";
    loading.value = false;
    return;
  } else if (confirmPassword.value != password.value) {
    errorMessage.value = "Hasła nie są identyczne";
    loading.value = false;
    return;
  }

  try {
    errorMessage.value = "";
    const request = axios.create({
      baseURL: `${prefix}/api`,
      timeout: 10000,
    });
    request.interceptors.request.use((config) => {
      config.headers.Authorization = token.toString().replace("\xa0", "+");
      return config;
    });
    await request.patch("/auth/reset-password", {
      password: password.value,
      confirmPassword: confirmPassword.value,
    });
    passwordReset.value = true;
  } catch (error: any) {
    errorMessage.value = "Token do zmiany hasła stracił ważność";
    console.log(error);
    loading.value = false;
  }
  loading.value = false;
};
</script>

<template>
  <div class="card">
    <div v-if="!passwordReset">
      <h2 class="pigment-green-text">Nie pamiętam hasła</h2>
      <div class="black-text">Podaj adres e-mail</div>
      <br />
      <form @submit.prevent="resetPasword">
        <input
          class="login-input gray-placeholder"
          v-model="password"
          placeholder="Nowe hasło"
          type="password"
        />
        <input
          class="login-input gray-placeholder"
          v-model="confirmPassword"
          placeholder="Powtórz hasło"
          type="password"
        />
        <br />
        <button type="submit" class="submit pigment-green">Zapisz</button>
      </form>
      <div v-if="loading">
        <img src="/src/assets/loading.gif" height="30" />
      </div>
      <div v-if="errorMessage" class="red-text">
        {{ errorMessage }}
      </div>
    </div>
    <div v-else>
      <h2 class="pigment-green-text">Hasło zostało zmienione</h2>
      <a href="/login">
        <button class="submit pigment-green">Powrót do logowania</button>
      </a>
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
.login-input {
  height: 4vh;
  width: 70%;
  border-radius: 10px;
  border: #e8e8e8 1px solid;
  background-color: #f6f6f6;
  color: rgb(0, 0, 0);
  padding-left: 10px;
  margin-top: 10px;
}
.back {
  float: left;
  margin: 0;
  cursor: pointer;
  -webkit-user-select: none; /* Safari */
  -ms-user-select: none; /* IE 10 and IE 11 */
  user-select: none; /* Standard syntax */
}
.back:hover {
  text-decoration: underline;
}
.back:active {
  color: purple;
}
</style>
