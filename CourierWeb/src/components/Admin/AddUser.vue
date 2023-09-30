<script setup lang="ts">
import { ref } from "vue";
const close_info = defineEmits(["closeModal"]);
const close = () => {
  close_info("closeModal");
};

const firstName = ref<string>("");
const lastName = ref<string>("");
const userName = ref<string>("");
const email = ref<string>("");
const password = ref<string>("");
</script>

<template>
  <div class="fog" @click.self="close">
    <div class="modal">
      <form class="register-form">
        <input
          class="register-input"
          type="text"
          placeholder="Imię"
          v-model="firstName"
        />
        <input
          class="register-input"
          type="text"
          placeholder="Nazwisko"
          v-model="lastName"
        />
        <input
          class="register-input"
          type="text"
          placeholder="Login"
          v-model="userName"
        />
        <input
          class="register-input"
          type="text"
          placeholder="Email"
          v-model="email"
        />
        <input
          class="register-input"
          type="password"
          placeholder="Hasło"
          v-model="password"
        />
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
.register-form {
  width: 40%;
  height: 90%;
  padding: 1%;
}
.register-input {
  height: 10%;
  width: 100%;
  margin: 1% 0;
}
</style>
