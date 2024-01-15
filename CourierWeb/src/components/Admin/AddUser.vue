<script setup lang="ts">
import { ref } from "vue";
import { authorized } from "../../main";
const emit = defineEmits(["closeModal", "fetchData"]);
const close = () => {
  emit("closeModal");
};
const props = defineProps<{
  role: string;
}>();
const firstName = ref<string>("");
const lastName = ref<string>("");
const userName = ref<string>("");
const email = ref<string>("");
const password = ref<string>("");
const confirmPassword = ref<string>("");
const phoneNumber = ref<string>("");

const loading = ref<boolean>(false);

const phoneErrorMessage = ref<string | null>(null);
const passwordErrorMessage = ref<string | null>(null);
const emailErrorMessage = ref<string | null>(null);

const validatePassword: () => boolean = () => {
  if (password.value.length < 8) return false;
  if (!/\d/.test(password.value)) return false;
  if (!/[A-Z]/.test(password.value)) return false;
  return true;
};

const validateEmail = (): boolean => {
  var validRegex =
    /^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$/;
  if (email.value.match(validRegex)) {
    emailErrorMessage.value = null;
    return true;
  }
  if (email.value.length == 0) {
    emailErrorMessage.value = "Podaj adres e-mail";
    return false;
  }
  emailErrorMessage.value = "Nieprawidłowy adres e-mail";
  return false;
};

const validateNumber: () => boolean = () => {
  const cleanedPhoneNumber: string = phoneNumber.value
    .replace(/\s/g, "")
    .replace("+", "");
  if (!/^\d+$/.test(cleanedPhoneNumber)) {
    phoneErrorMessage.value = "Nieprawidłowy numer telefonu";
    return false;
  }
  if (![9, 11].includes(cleanedPhoneNumber.length)) {
    phoneErrorMessage.value = "Nieprawidłowy numer telefonu";
    return false;
  }
  if (cleanedPhoneNumber.length === 11 && !/^48/.test(cleanedPhoneNumber)) {
    phoneErrorMessage.value = "Nieprawidłowy numer telefonu";
    return false;
  }
  phoneErrorMessage.value = null;
  return true;
};

const submit = async () => {
  if (!validatePassword()) {
    passwordErrorMessage.value =
      "Hasło powinno składać się z co najmniej 8 znaków i zawierać przynajmniej jedną wielkią literę i jedną cyfrę";
    validateNumber();
    validateEmail();
    return;
  }
  if (password.value != confirmPassword.value) {
    passwordErrorMessage.value = "Hasła nie są identyczne";
    validateNumber();
    validateEmail();
    return;
  }
  if (!validateNumber()) {
    validateEmail();
    return;
  }
  if (!validateEmail()) {
    return;
  }
  passwordErrorMessage.value = null;
  emailErrorMessage.value = null;
  phoneErrorMessage.value = null;
  try {
    loading.value = true;
    const url =
      props.role == "Dispatcher"
        ? "/admin/add-dispatcher"
        : "/admin/add-courier";
    console.log("test");
    await authorized.post(url, {
      firstName: firstName.value,
      lastName: lastName.value,
      userName: userName.value,
      email: email.value,
      password: password.value,
      confirmPassword: confirmPassword.value,
      phoneNumber: phoneNumber.value,
    });
    loading.value = false;
  } catch (error: any) {
    console.log(error);
    loading.value = false;
  }
  emit("fetchData");
  close();
};
</script>

<template>
  <div class="fog">
    <div class="modal">
      <h2 class="pigment-green-text center">
        {{ props.role == "Dispatcher" ? "Dodaj dyspozytora" : "Dodaj kuriera" }}
      </h2>
      <form @submit.prevent="submit" class="flex-col">
        <input
          class="add-user-input center"
          type="text"
          placeholder="Imię"
          v-model="firstName"
        />
        <input
          class="add-user-input center"
          type="text"
          placeholder="Nazwisko"
          v-model="lastName"
        />
        <input
          class="add-user-input center"
          type="text"
          placeholder="Login"
          v-model="userName"
        />
        <input
          class="add-user-input center"
          type="text"
          placeholder="Email"
          v-model="email"
        />
        <span class="red-text center-text" v-if="emailErrorMessage">
          {{ emailErrorMessage }}
        </span>
        <input
          class="add-user-input center"
          type="password"
          placeholder="Hasło"
          v-model="password"
        />
        <input
          class="add-user-input center"
          type="password"
          placeholder="Powtórz hasło"
          v-model="confirmPassword"
        />
        <span class="red-text center-text" v-if="passwordErrorMessage">
          {{ passwordErrorMessage }}
        </span>
        <input
          class="add-user-input center"
          type="text"
          placeholder="Telefon"
          v-model="phoneNumber"
        />
        <span class="red-text center-text" v-if="phoneErrorMessage">
          {{ phoneErrorMessage }}
        </span>
        <div class="center mt-10 mb-10">
          <button class="submit center spacing" @click="close">Anuluj</button>
          <button type="submit" class="submit center pigment-green spacing">
            Zapisz
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<style scoped>
.modal {
  background-color: white;
  width: 30vw;
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  border-radius: 12px;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.25);
  flex-direction: column;
  display: flex;
  max-height: 75vh;
  overflow-y: auto;
}
.modal::-webkit-scrollbar {
  width: 10px;
}
.modal::-webkit-scrollbar-track {
  border-radius: 10px;
  background: #bdbdbd;
}
.modal::-webkit-scrollbar-thumb {
  background: #15ab54;
  border-radius: 10px;
}
.modal::-webkit-scrollbar-thumb:hover {
  background: #129448;
}
.center-text {
  text-align: center;
}
.add-user-form {
  width: 40%;
  height: 90%;
  padding: 1%;
}
.add-user-input {
  border-radius: 10px;
  height: 30px;
  width: 90%;
  margin-top: 1vh !important;
  background-color: #f6f6f6;
  border: solid 2px #e8e8e8;
  color: black;
}
.spacing {
  margin: 0 20px !important;
}
</style>
