<script setup lang="ts">
import { EmailSent, ForgotPassword } from ".";
import { onBeforeMount, ref } from "vue";
import { router, unauthorized, user, loading } from "../../main";

const login = ref("");
const password = ref("");
const remember_me = ref(false);

onBeforeMount(() => {
  if (user.isLoggedIn) router.push("/");
});

const signIn = async () => {
  try {
    loading.value = true;
    const res = await unauthorized.post("/auth/login", {
      login: login.value,
      password: password.value,
    });
    localStorage.setItem("token", res.data.accessToken);
    if (remember_me.value) {
      localStorage.setItem("expireDate", res.data.expireDate);
    } else {
      let time = new Date(Date.now());
      time.setTime(time.getTime() + 2 * 60 * 60 * 1000);
      localStorage.setItem("expireDate", time.toString());
    }
    localStorage.setItem("user", res.data.user);
    localStorage.setItem("roles", JSON.stringify(res.data.roles));
    user.name = res.data.user;
    user.isLoggedIn = true;
    user.roles = res.data.roles;
    loading.value = false;
    if (user.roles.includes("Admin")) {
      router.push("/administration");
    } else router.push("/");
  } catch (error: any) {
    if (error.request.status == 400) {
      errorMessage.value = "Podaj dane logowania";
    } else if (error.request.status === 403) {
      confirmEmail.value = true;
    } else if (error.request.status === 401) {
      errorMessage.value = "Niepoprawne dane logowania";
    }
    console.log(error);
    loading.value = false;
  }
};

const forgotPassword = ref<boolean>(false);

const errorMessage = ref<string>("");

const confirmEmail = ref<boolean>(false);

const back = () => {
  forgotPassword.value = false;
  errorMessage.value = "";
};
</script>

<template>
  <div class="card">
    <div v-if="!confirmEmail && !forgotPassword">
      <h2 class="pigment-green-text">Zaloguj się</h2>
      <form class="login-form" @submit.prevent="signIn">
        <input
          class="login-input gray-placeholder"
          v-model="login"
          placeholder="Login"
          type="text"
        />
        <input
          class="login-input gray-placeholder"
          v-model="password"
          placeholder="Hasło"
          type="password"
        />
        <div>
          <input id="remember-me" v-model="remember_me" type="checkbox" />
          <label class="black-text" for="rememberMe">Zapamiętaj mnie</label>
        </div>

        <button class="submit center mt-10" type="submit" @click="signIn">
          Zaloguj
        </button>
        <div v-if="errorMessage" class="red-text">
          {{ errorMessage }}
        </div>
        <a class="pointer" @click="forgotPassword = true">Nie pamiętam hasła</a>
      </form>
    </div>
    <EmailSent
      v-else-if="confirmEmail && !forgotPassword"
      :email="login"
      :forgotPassword="false"
    />
    <ForgotPassword v-else @back="back()" />
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

.login-form {
  display: flex;
  flex-direction: column;
  padding: 5%;
  width: 70%;
  margin: auto;
}
.login-input {
  margin: 2% 0;
  height: 4vh;
  border-radius: 10px;
  border: #e8e8e8 1px solid;
  background-color: #f6f6f6;
  color: rgb(0, 0, 0);
  padding-left: 10px;
}
#remember-me {
  transform: scale(1.2);
}

.pointer {
  cursor: pointer;
  -webkit-user-select: none; /* Safari */
  -ms-user-select: none; /* IE 10 and IE 11 */
  user-select: none; /* Standard syntax */
}
.pointer:hover {
  text-decoration: underline;
}
.pointer:active {
  color: purple;
}
</style>
