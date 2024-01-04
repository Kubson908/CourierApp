<script setup lang="ts">
import { onBeforeMount, ref } from "vue";
import { authorized, router, user } from "../main";
import { ChangePassword } from ".";

const loading = ref<boolean>(false);
const profileInfo = ref<{ email: string; phoneNumber: string }>();

const fetchData = async () => {
  loading.value = true;
  try {
    const res = await authorized.get("/auth/get-profile-info");
    profileInfo.value = res.data;
  } catch {}
  loading.value = false;
};

onBeforeMount(async () => {
  await fetchData();
});

const editNumber = ref<boolean>(false);
const newNumber = ref<string>("");
const password = ref<string>("");
const errorMessage = ref<string | null>(null);

const validateNumber: () => boolean = () => {
  const cleanedPhoneNumber: string = newNumber.value
    .replace(/\s/g, "")
    .replace("+", "");
  if (!/^\d+$/.test(cleanedPhoneNumber)) {
    return false;
  }
  if (![9, 11].includes(cleanedPhoneNumber.length)) {
    return false;
  }
  if (cleanedPhoneNumber.length === 11 && !/^48/.test(cleanedPhoneNumber)) {
    return false;
  }

  return true;
};

const submitNumber = async () => {
  if (newNumber.value.length == 0) {
    errorMessage.value = "Podaj numer telefonu";
    return;
  }
  if (!validateNumber()) {
    errorMessage.value = "Nieprawidłowy numer telefonu";
    return;
  }
  errorMessage.value = null;
  loading.value = true;
  try {
    const res = await authorized.patch("/auth/change-phone-number", {
      phoneNumber: newNumber.value,
      password: password.value,
    });
    if (res.status == 200) {
      loading.value = false;
      editNumber.value = false;
      newNumber.value = "";
      password.value = "";
      await fetchData();
      return;
    }
  } catch (error: any) {
    if (error.response.data.message == "Invalid password")
      errorMessage.value = "Nieprawidłowe hasło";
    loading.value = false;
  }
};

const changePassword = ref<boolean>(false);
</script>

<template>
  <div class="card">
    <div v-if="!loading && !editNumber" class="black-text">
      <h2 class="pigment-green-text">
        Konto
        {{
          !user.roles.includes("Admin")
            ? !user.roles.includes("Dispatcher")
              ? "użytkownika"
              : "dyspozytora"
            : "administratora"
        }}
      </h2>
      <div v-if="!user.roles.includes('Admin')">
        <div class="profile-section">
          {{ user.name }}
        </div>
        <div class="profile-section">Email: {{ profileInfo?.email }}</div>
        <div class="profile-section">Tel: {{ profileInfo?.phoneNumber }}</div>
        <div class="profile-section">
          <button class="submit black" @click="editNumber = true">
            Zmień numer telefonu
          </button>
          <button class="submit black" @click="changePassword = true">
            Zmień hasło
          </button>
          <button class="submit pigment-green" @click="router.push('/history')">
            Archiwum zamówień
          </button>
        </div>
      </div>
    </div>
    <div v-else-if="editNumber && !loading" class="cover">
      <h2 class="pigment-green-text">Zmień numer telefonu</h2>
      <input
        v-model="newNumber"
        class="phone-input"
        placeholder="Numer telefonu"
      />
      <input
        type="password"
        v-model="password"
        class="phone-input"
        placeholder="Hasło"
      />
      <div>
        <button
          class="submit black"
          @click="
            editNumber = false;
            errorMessage = null;
            newNumber = '';
            password = '';
          "
        >
          Anuluj
        </button>
        <button class="submit pigment-green" @click="submitNumber">
          Zapisz
        </button>
      </div>
      <span class="red-text" v-if="errorMessage">{{ errorMessage }}</span>
    </div>
    <div v-else>
      <img src="/src/assets/loading.gif" class="loading-icon" />
    </div>
    <ChangePassword v-if="changePassword" @close="changePassword = false" />
  </div>
</template>

<style scoped>
.card {
  background-color: white;
  width: 20vw;
  min-height: 520px !important;
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  border-radius: 12px;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.25);
  flex-direction: column;
  display: flex;
  max-height: 75vh;
  padding: 0;
}
.cover {
  position: absolute;
  width: 20vw;
  background-color: white;
  border-radius: 12px;
  color: black;
  display: block;
  text-align: center;
}
.loading-icon {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
}
.profile-section {
  font-size: large;
  margin-top: 10px;
  display: flex;
  flex-direction: column;
  align-items: center;
}
.phone-input {
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
