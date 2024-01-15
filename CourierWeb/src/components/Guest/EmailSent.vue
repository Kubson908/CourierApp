<script setup lang="ts">
import { ref } from "vue";
import { unauthorized } from "../../main";
import { ForgotPassword } from ".";

const props = defineProps<{
  email: string;
  forgotPassword: boolean;
}>();

const emailSent = ref<boolean>(false);
const loading = ref<boolean>(false);
const error = ref<boolean>(false);
const resendEmail = async () => {
  emailSent.value = false;
  loading.value = true;
  error.value = false;
  try {
    await unauthorized.get(
      ForgotPassword ? "/auth/reset-password-email" : "/auth/resend-email",
      {
        params: {
          email: props.email,
        },
      }
    );
    emailSent.value = true;
  } catch {
    error.value = true;
  }
  loading.value = false;
};
</script>

<template>
  <div>
    <h2 class="pigment-green-text">
      {{ !forgotPassword ? "Zweryfikuj adres e-mail" : "Zmiana hasła" }}
    </h2>
    <div class="black-text">
      {{
        !forgotPassword
          ? "Na podany adres e-mail został wysłany link do aktywacji konta."
          : "Na podany adres e-mail został wysłany link do zmiany hasła"
      }}
    </div>
    <div class="black-text">Sprawdź również folder spam.</div>
    <br />
    <div class="black-text">Wiadomość nie dotarła?</div>
    <a class="pointer" @click="resendEmail">Wyślij ponownie</a>
    <div v-if="error" class="red-text">Błąd podczas wysyłania wiadomości</div>
    <div v-if="!loading && emailSent" class="pigment-green-text">
      Wiadomość została wysłana
    </div>
    <div v-else-if="loading">
      <img src="/src/assets/loading.gif" height="30" />
    </div>
  </div>
</template>

<style scoped>
.pointer {
  cursor: pointer;
  -webkit-user-select: none; /* Safari */
  -ms-user-select: none; /* IE 10 and IE 11 */
  user-select: none; /* Standard syntax */
  color: #15ab54;
}
.pointer:hover {
  text-decoration: underline;
}
.pointer:active {
  color: #0f7238;
}
</style>
