<script setup lang="ts">
import { ref } from "vue";
import { authorized } from "../../main";

const props = defineProps<{
  user: any;
}>();

const loading = ref<boolean>(false);
const errorMessage = ref<string | null>(null);

const firstName = ref<string>(props.user.firstName);
const lastName = ref<string>(props.user.lastName);
const userName = ref<string>(props.user.userName);
const email = ref<string>(props.user.email);
const phoneNumber = ref<string>(props.user.phoneNumber);

const emit = defineEmits(["closeModal", "fetchData"]);
const close = () => {
  emit("closeModal");
};

const submit = async () => {
  loading.value = true;
  const url: string =
    props.user.role == "Dispatcher"
      ? "/admin/update-dispatcher/"
      : "/admin/update-courier/";
  try {
    const res = await authorized.patch(url + props.user.id, {
      firstName: firstName.value,
      lastName: lastName.value,
      userName: userName.value,
      email: email.value,
      phoneNumber: phoneNumber.value,
    });
    if (res.status == 200) {
      loading.value = false;
      emit("fetchData");
      close();
    }
  } catch {
    loading.value = false;
  }
};

const remove = ref<boolean>(false);

const deleteWorker = async () => {
  loading.value = true;
  const url: string =
    props.user.role == "Dispatcher"
      ? "/admin/delete-dispatcher/"
      : "/admin/delete-courier/";
  try {
    var res = await authorized.delete(url + props.user.id);
    if (res.status == 200) {
      loading.value = false;
      emit("fetchData");
      close();
    }
  } catch {
    loading.value = false;
    //TODO: mozna zrobic snackbar z komunikatem o bledzie
  }
};

const passwordEdit = ref<boolean>(false);
const newPassword = ref<string>("");
const repeatPassword = ref<string>("");

const validatePassword: () => boolean = () => {
  if (newPassword.value.length < 8) return false;
  if (!/\d/.test(newPassword.value)) return false;
  if (!/[A-Z]/.test(newPassword.value)) return false;
  return true;
};

const resetPassword = async () => {
  if (newPassword.value == "" || repeatPassword.value == "") {
    errorMessage.value = "Podaj nowe hasło dwukrotnie";
    return;
  }
  if (!validatePassword()) {
    errorMessage.value =
      "Hasło powinno składać się z co najmniej 8 znaków i zawierać przynajmniej jedną wielkią literę i jedną cyfrę";
    return;
  }
  if (newPassword.value != repeatPassword.value) {
    errorMessage.value = "Hasła muszą być identyczne";
    return;
  }
  loading.value = true;
  errorMessage.value = null;
  const url: string =
    props.user.role == "Dispatcher"
      ? "/admin/reset-dispatcher-password/"
      : "/admin/reset-courier-password/";
  try {
    const res = await authorized.patch(url + props.user.id, {
      password: newPassword.value,
      confirmPassword: repeatPassword.value,
    });
    if (res.status == 200) {
      loading.value = false;
      emit("fetchData");
      close();
    }
  } catch {
    loading.value = false;
  }
};
</script>

<template>
  <div class="fog">
    <table class="container">
      <tr>
        <td class="column">
          <!-- <button
            v-if="!remove"
            class="submit change-password"
            @click="passwordEdit = true"
          >
            Hasło
          </button> -->
        </td>
        <td class="middle-column">
          <div class="modal">
            <h2 class="pigment-green-text center">Edycja danych użytkownika</h2>
            <form @submit.prevent="submit" class="flex-col">
              <span class="label">Imię</span>
              <input class="edit-user-input center" v-model="firstName" />
              <span class="label">Nazwisko</span>
              <input class="edit-user-input center" v-model="lastName" />
              <span class="label">Login</span>
              <input class="edit-user-input center" v-model="userName" />
              <span class="label">E-mail</span>
              <input class="edit-user-input center" v-model="email" />
              <span class="label">Telefon</span>
              <input class="edit-user-input center" v-model="phoneNumber" />
            </form>
            <div class="center mt-10 mb-10">
              <button class="submit center spacing" @click="close">
                Anuluj
              </button>
              <button
                type="submit"
                class="submit center pigment-green spacing"
                @click="submit"
              >
                Zapisz
              </button>
            </div>
            <div v-if="remove" class="cover">
              <h2 class="red-text">Czy napewno chcesz usunąć pracownika?</h2>
              <span
                >{{
                  props.user.role == "Dispatcher" ? "Dyspozytor" : "Kurier"
                }}: {{ firstName + " " + lastName }}
              </span>
              <div>
                <button class="black" @click="remove = false">Anuluj</button>
                <button class="red" @click="deleteWorker">Usuń</button>
              </div>
            </div>
            <div v-if="passwordEdit" class="cover">
              <h2 class="pigment-green-text">
                Podaj nowe hasło dla pracownika
              </h2>
              <span>
                {{ props.user.role == "Dispatcher" ? "Dyspozytor" : "Kurier" }}:
                {{ firstName + " " + lastName }}
              </span>
              <br />
              <br />
              <input
                class="edit-user-input center submit"
                v-model="newPassword"
                placeholder="Nowe hasło"
              />
              <br />
              <input
                class="edit-user-input center submit"
                v-model="repeatPassword"
                placeholder="Powtórz hasło"
              />
              <br />
              <span v-if="errorMessage" class="red-text">{{
                errorMessage
              }}</span>
              <div>
                <button class="black" @click="passwordEdit = false">
                  Anuluj
                </button>
                <button class="pigment-green" @click="resetPassword">
                  Zapisz
                </button>
              </div>
            </div>
            <div v-if="loading" class="cover">
              <img src="/src/assets/loading.gif" class="loading-icon" />
            </div>
          </div>
        </td>
        <td class="column">
          <button
            v-if="!remove && !passwordEdit"
            class="submit delete"
            @click="
              remove = true;
              passwordEdit = false;
            "
          >
            Usuń
          </button>
          <button
            v-if="!remove && !passwordEdit"
            class="submit change-password"
            @click="
              passwordEdit = true;
              remove = false;
            "
          >
            Zmień hasło
          </button>
        </td>
      </tr>
    </table>
  </div>
</template>

<style scoped>
.label {
  margin-bottom: 0;
  font-size: small;
  color: rgb(85, 85, 85);
  margin-left: 5%;
}
.loading-icon {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
}
.cover {
  position: absolute;
  width: 30vw;
  height: 100%;
  background-color: white;
  border-radius: 12px;
  color: black;
  display: block;
  text-align: center;
}
.middle-column {
  padding: 0;
}
.column {
  display: flex;
  flex-direction: column;
  padding: 0;
  width: 20vw;
}
.change-password {
  margin: 0 !important;
  border-radius: 0 12px 12px 0;
  width: 100px !important;
  height: 50px;
  padding: 0;
  background-color: rgb(0, 0, 0);
}
.delete {
  background-color: rgb(170, 0, 0);
  margin: 0 !important;
  padding: 0;
  border-radius: 0 12px 12px 0;
  width: 100px !important;
  height: 50px;
  margin-bottom: 5px !important;
}
.container {
  width: 30vw;
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  border-collapse: collapse;
  border-spacing: 0;
}
.modal {
  background-color: white;
  width: 30vw;
  border-radius: 12px 0 12px 12px;
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
.edit-user-input {
  border-radius: 10px;
  height: 30px;
  width: 90%;
  margin-bottom: 1vh !important;
  background-color: #f6f6f6;
  border: solid 2px #e8e8e8;
  color: black;
}
.spacing {
  margin: 0 20px !important;
}
</style>
