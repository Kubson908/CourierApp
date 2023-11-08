<script setup lang="ts">
import { ref } from "vue";
import { loading, unauthorized } from "../../main";
const emit = defineEmits(["closeModal", "fetchData"]);
const close = () => {
  emit("closeModal");
};
const role = ref<string>("");
const firstName = ref<string>("");
const lastName = ref<string>("");
const userName = ref<string>("");
const email = ref<string>("");
const password = ref<string>("");
const confirmPassword = ref<string>("");
const phoneNumber = ref<string>("");

const submit = async () => {
  // const valid = ((await data) as any).valid;
  // if (!valid) return;
  if (password.value != confirmPassword.value) return;
  try {
    loading.value = true;
    const url =
      role.value == "Dispatcher" ? "/auth/add-dispatcher" : "/auth/add-courier";
    await unauthorized.post(url, {
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
  <div class="fog" @click.self="close">
    <div class="modal">
      <form @submit.prevent="submit" class="add-user-form">
        <select v-model="role" class="add-user-input">
          <option disabled value="">--Rola--</option>
          <option value="Dispatcher">Dyspozytor</option>
          <option value="Courier">Kurier</option>
        </select>
        <input
          class="add-user-input"
          type="text"
          placeholder="Imię"
          v-model="firstName"
        />
        <input
          class="add-user-input"
          type="text"
          placeholder="Nazwisko"
          v-model="lastName"
        />
        <input
          class="add-user-input"
          type="text"
          placeholder="Login"
          v-model="userName"
        />
        <input
          class="add-user-input"
          type="text"
          placeholder="Email"
          v-model="email"
        />
        <input
          class="add-user-input"
          type="password"
          placeholder="Hasło"
          v-model="password"
        />
        <input
          class="add-user-input"
          type="password"
          placeholder="Powtórz hasło"
          v-model="confirmPassword"
        />
        <input
          class="add-user-input"
          type="text"
          placeholder="Telefon"
          v-model="phoneNumber"
        />
        <input type="submit" value="Zapisz" />
      </form>
    </div>
  </div>
</template>

<style scoped>
.fog {
  background-color: rgba(83, 83, 83, 0.789);
  position: fixed;
  z-index: 1;
  width: 100%;
  height: 100%;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
}
.modal {
  width: 40vw;
  height: 40vh;
  padding: 0 20px;
  z-index: 1;
  background: #1b1919;
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  margin: auto;
}
.add-user-form {
  width: 40%;
  height: 90%;
  padding: 1%;
}
.add-user-input {
  height: 10%;
  width: 100%;
  margin: 1% 0;
}
</style>
