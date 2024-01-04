<script setup lang="ts">
import { ref } from "vue";
import { authorized } from "../main";

const emit = defineEmits(["close"]);

const oldPassword = ref<string>("");
const newPassword = ref<string>("");
const confirmNewPassword = ref<string>("");

const loading = ref<boolean>(false);
const errorMessage = ref<string | null>(null);

const validatePassword: () => boolean = () => {
  if (newPassword.value.length < 8) return false;
  if (!/\d/.test(newPassword.value)) return false;
  if (!/[A-Z]/.test(newPassword.value)) return false;
  return true;
};

const submit = async () => {
  if (
    oldPassword.value.length == 0 ||
    newPassword.value.length == 0 ||
    confirmNewPassword.value.length == 0
  ) {
    errorMessage.value = "Wypełnij wszystkie pola";
    return;
  }
  if (!validatePassword()) {
    errorMessage.value =
      "Hasło powinno składać się z co najmniej 8 znaków i zawierać przynajmniej jedną wielkią literę i jedną cyfrę";
    return;
  }
  if (newPassword.value != confirmNewPassword.value) {
    errorMessage.value = "Hasła nie są identyczne";
    return;
  }
  errorMessage.value = null;
  loading.value = true;
  try {
    var res = await authorized.patch("/auth/change-password", {
      oldPassword: oldPassword.value,
      newPassword: newPassword.value,
    });
    if ((res.status = 200)) {
      emit("close");
    }
  } catch (error: any) {
    console.log(error);
    if (error.request.status == 401)
      errorMessage.value = "Podano nieprawidłowe aktualne hasło";
    else if (error.request.status == 500)
      errorMessage.value = "Wystąpił błąd serwera";
    else if (error.request.status == 0)
      errorMessage.value = "Brak połączenia z serwerem";
    else errorMessage.value = "Wystąpił błąd";
  }
  loading.value = false;
};
</script>

<template>
  <div class="cover">
    <div v-if="!loading">
      <h2 class="pigment-green-text">Zmiana hasła</h2>
      <input
        type="password"
        v-model="oldPassword"
        class="password-input"
        placeholder="Aktualne hasło"
      />
      <input
        type="password"
        v-model="newPassword"
        class="password-input"
        placeholder="Nowe hasło"
      />
      <input
        type="password"
        v-model="confirmNewPassword"
        class="password-input"
        placeholder="Powtórz nowe hasło"
      />
      <div>
        <button class="submit black" @click="emit('close')">Anuluj</button>
        <button class="submit pigment-green" @click="submit">Zapisz</button>
      </div>
      <span class="red-text" v-if="errorMessage">
        {{ errorMessage }}
      </span>
    </div>
    <div v-else>
      <img src="/src/assets/loading.gif" class="loading-icon" />
    </div>
  </div>
</template>

<style scoped>
.cover {
  position: absolute;
  width: 20vw;
  height: 100%;
  background-color: white;
  border-radius: 12px;
  color: black;
  display: block;
  text-align: center;
}
.password-input {
  border-radius: 10px;
  height: 30px;
  width: 60%;
  margin-top: 10%;
  background-color: #f6f6f6;
  border: solid 2px #e8e8e8;
  color: black;
  font-size: 20px;
}
</style>
