<script setup lang="ts">
import { EmailSent } from ".";
import { ref } from "vue";
import { unauthorized } from "../../main";

const emit = defineEmits(["back"]);

const email = ref<string>("");
const loading = ref<boolean>(false);
const errorMessage = ref<string>("");
const emailSent = ref<boolean>(false);

const validateEmail = (): boolean => {
  var validRegex =
    /^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$/;
  if (email.value.match(validRegex)) return true;
  return false;
};

const sendEmail = async () => {
  if (email.value == "") {
    errorMessage.value = "Podaj adres e-mail";
    loading.value = false;
    return;
  }
  loading.value = true;
  if (!validateEmail()) {
    errorMessage.value = "Niepoprawny adres e-mail";
    loading.value = false;
    return;
  }
  try {
    errorMessage.value = "";
    await unauthorized.get("/auth/reset-password-email", {
      params: {
        email: email.value,
      },
    });
    emailSent.value = true;
  } catch (error: any) {
    if (error.request.status == 423) {
      errorMessage.value =
        "Aby zmienić hasło do konta pracownika skontaktuj się z administratorem";
    } else {
      errorMessage.value = "Nie znaleziono użytkownika";
    }
    console.log(error);
    loading.value = false;
  }
  loading.value = false;
};
</script>

<template>
  <div>
    <div v-if="!emailSent">
      <h2 class="pigment-green-text">Nie pamiętam hasła</h2>
      <div class="black-text">Podaj adres e-mail</div>
      <br />
      <form @submit.prevent="sendEmail">
        <input
          class="login-input gray-placeholder"
          v-model="email"
          placeholder="E-mail"
          type="text"
        />
        <br />
        <button class="submit" @click="emit('back')">Wróć</button>
        <button type="submit" class="submit pigment-green">Wyślij</button>
      </form>
      <div v-if="loading">
        <img src="/src/assets/loading.gif" height="30" />
      </div>
      <div v-if="errorMessage" class="red-text">
        {{ errorMessage }}
      </div>
    </div>
    <EmailSent v-else :email="email" :forgotPassword="true" />
  </div>
</template>

<style scoped>
.login-input {
  height: 4vh;
  width: 70%;
  border-radius: 10px;
  border: #e8e8e8 1px solid;
  background-color: #f6f6f6;
  color: rgb(0, 0, 0);
  padding-left: 10px;
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
