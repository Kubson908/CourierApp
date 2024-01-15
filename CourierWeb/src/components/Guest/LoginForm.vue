<script setup lang="ts">
import { EmailSent, ForgotPassword } from ".";
import { onBeforeMount, onBeforeUnmount, ref } from "vue";
import { router, unauthorized, user, loading } from "../../main";

const login = ref("");
const password = ref("");
const rememberMe = ref(false);
const errorMessage = ref<string | null>(null);

onBeforeMount(() => {
  if (user.isLoggedIn) router.push("/");
});

const signIn = async () => {
  if (login.value.length == 0 || password.value.length == 0) {
    errorMessage.value = "Podaj dane logowania";
    return;
  }
  errorMessage.value = null;
  try {
    loading.value = true;
    const res = await unauthorized.post("/auth/login", {
      login: login.value,
      password: password.value,
    });
    localStorage.setItem("token", res.data.accessToken);
    if (rememberMe.value) {
      localStorage.setItem("rememberMe", "true");
      localStorage.setItem("expireDate", res.data.expireDate);
    } else {
      let time = new Date(Date.now());
      time.setTime(time.getTime() + 2 * 60 * 60 * 1000);
      localStorage.setItem("expireDate", time.toString());
      localStorage.setItem("rememberMe", "false");
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

const confirmEmail = ref<boolean>(false);

const back = () => {
  forgotPassword.value = false;
  errorMessage.value = "";
};

onBeforeUnmount(() => {
  password.value = "";
  errorMessage.value = null;
  login.value = "";
});
</script>

<template>
  <div class="card">
    <div v-if="!confirmEmail && !forgotPassword">
      <h2 class="pigment-green-text">Zaloguj się</h2>
      <form class="login-form" @submit.prevent="signIn">
        <input
          name="login"
          class="login-input gray-placeholder"
          v-model="login"
          placeholder="Login"
          type="text"
        />
        <input
          name="password"
          class="login-input gray-placeholder"
          v-model="password"
          placeholder="Hasło"
          type="password"
        />
        <span v-if="errorMessage" class="red-text">{{ errorMessage }}</span>
        <div>
          <input
            name="remember_me"
            id="remember-me"
            v-model="rememberMe"
            type="checkbox"
          />
          <label class="black-text" for="rememberMe">Zapamiętaj mnie</label>
        </div>

        <button
          class="submit pigment-green center mt-10"
          type="submit"
          @click="signIn"
        >
          Zaloguj
        </button>
        <a class="pointer" @click="forgotPassword = true">Nie pamiętam hasła</a>
      </form>
    </div>
    <EmailSent
      v-else-if="confirmEmail && !forgotPassword"
      :email="login"
      :forgotPassword="false"
    />
    <ForgotPassword v-else @back="back()" />
    <div v-if="loading" class="loading">
      <img src="/src/assets/loading.gif" class="loading-icon" />
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
  color: #15ab54;
}
.pointer:hover {
  text-decoration: underline;
}
.pointer:active {
  color: #0f7238;
}
.loading {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  height: 100%;
  padding: 0;
  width: 100%;
  display: flex;
  background-color: white;
  border-radius: 12px;
}
.loading-icon {
  align-self: center;
  margin: auto;
  height: 150px;
}
</style>
