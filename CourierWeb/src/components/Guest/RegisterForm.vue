<script setup lang="ts">
import { EmailSent } from ".";
import { ref } from "vue";
import { unauthorized } from "../../main";

const firstName = ref<string>("");
const lastName = ref<string>("");
const email = ref<string>("");
const password = ref<string>("");
const confirmPassword = ref<string>("");
const phoneNumber = ref<string>("");
// TODO: dodac rules do hasla
const submit = async () => {
  if (password.value != confirmPassword.value) return;
  try {
    const res = await unauthorized.post("/auth/register-customer", {
      firstName: firstName.value,
      lastName: lastName.value,
      email: email.value,
      password: password.value,
      confirmPassword: confirmPassword.value,
      phoneNumber: phoneNumber.value,
    });
    if (res.status < 300) registered.value = true;
  } catch (error: any) {
    console.log(error);
  }
};

const registered = ref<boolean>(false);
</script>

<template>
  <div class="card">
    <div v-if="!registered">
      <h2 class="pigment-green-text">Załóż konto</h2>
      <form @submit.prevent="submit" class="register-form">
        <input
          type="text"
          v-model="firstName"
          class="register-input gray-placeholder"
          placeholder="Imię"
        />
        <input
          type="text"
          v-model="lastName"
          class="register-input gray-placeholder"
          placeholder="Nazwisko"
        />
        <input
          type="text"
          v-model="email"
          class="register-input gray-placeholder"
          placeholder="Email"
        />
        <input
          type="text"
          v-model="phoneNumber"
          class="register-input gray-placeholder"
          placeholder="Telefon"
        />
        <input
          type="password"
          v-model="password"
          class="register-input gray-placeholder"
          placeholder="Hasło"
        />
        <input
          type="password"
          v-model="confirmPassword"
          class="register-input gray-placeholder"
          placeholder="Powtórz hasło"
        />
        <button type="submit" class="submit center">Zarejestruj</button>
      </form>
    </div>
    <EmailSent v-else :email="email" :forgotPassword="false" />
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
.register-form {
  display: flex;
  flex-direction: column;
  padding: 5%;
  width: 70%;
  margin: auto;
}
.register-input {
  margin: 2% 0;
  height: 4vh;
  border-radius: 10px;
  border: #e8e8e8 1px solid;
  background-color: #f6f6f6;
  color: rgb(0, 0, 0);
  padding-left: 10px;
}
</style>
