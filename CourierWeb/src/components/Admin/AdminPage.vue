<script setup lang="ts">
import { onBeforeMount, ref } from "vue";
import { authorized, loading } from "../../main";
import { UserCard, EditUser, AddUser } from ".";
import { Courier } from "../../typings";

const dispatchers = ref<any>(null);
const couriers = ref<Array<Courier>>();
const error = ref<boolean>(false);

const fetchData = async () => {
  loading.value = true;
  try {
    const getDispatchers = await authorized.get("/admin/get-dispatchers");
    dispatchers.value = getDispatchers.data.map((dispatcher: any) => {
      return {
        id: dispatcher.id,
        firstName: dispatcher.firstName,
        lastName: dispatcher.lastName,
        userName: dispatcher.userName,
        email: dispatcher.email,
        phoneNumber: dispatcher.phoneNumber,
        role: "Dispatcher",
      };
    });
    const getCouriers = await authorized.get("/admin/get-couriers");
    couriers.value = getCouriers.data.map((courier: Courier) => {
      courier.role = "Courier";
      return courier;
    });
    loading.value = false;
  } catch {
    loading.value = false;
    error.value = true;
  }
};

onBeforeMount(async () => {
  await fetchData();
});

const selectedUser = ref(null);
const showEdit = ref(false);
const edit = (user: any) => {
  selectedUser.value = user;
  showEdit.value = true;
};
const showAddDispatcher = ref(false);
const showAddCourier = ref(false);
</script>

<template>
  <div v-if="!loading && !error">
    <h1 class="pigment-green-text">Dyspozytorzy</h1>
    <div class="adminDiv">
      <UserCard
        v-for="dispatcher in dispatchers"
        :user="dispatcher"
        @edit="edit(dispatcher)"
      />
      <button
        class="addButton submit pigment-green"
        @click="showAddDispatcher = true"
      >
        Dodaj
      </button>
    </div>

    <teleport to="body">
      <EditUser
        :user="selectedUser"
        v-if="showEdit"
        @closeModal="showEdit = false"
        @fetchData="fetchData"
      />
      <AddUser
        v-if="showAddDispatcher"
        role="Dispatcher"
        @closeModal="showAddDispatcher = false"
        @fetchData="fetchData"
      />
    </teleport>
    <hr class="divider" />
    <h1 class="pigment-green-text">Kurierzy</h1>
    <div class="adminDiv">
      <UserCard
        v-for="courier in couriers"
        :user="courier"
        @edit="edit(courier)"
      />
      <button
        class="addButton submit pigment-green"
        @click="showAddCourier = true"
      >
        Dodaj
      </button>
    </div>

    <teleport to="body">
      <EditUser
        :user="selectedUser"
        v-if="showEdit"
        @closeModal="showEdit = false"
        @fetchData="fetchData"
      />
      <AddUser
        v-if="showAddCourier"
        role="Courier"
        @closeModal="showAddCourier = false"
        @fetchData="fetchData"
      />
    </teleport>
    <br />
  </div>
  <div v-else-if="loading" class="background">
    <img src="/src/assets/loading.gif" class="loading" />
  </div>
  <div v-else class="red-text">
    <h1>Nie udało się pobrać danych</h1>
  </div>
</template>

<style scoped>
.background {
  width: 100vw;
  height: 100vh;
  background: #ffffff;
  position: fixed;
}
.adminDiv {
  color: black;
  display: grid;
  gap: 2vh;
  grid-template-columns: 1fr 1fr 1fr 1fr;
  width: 70%;
  margin: auto;
  margin-top: 10px;
}
.addButton {
  width: fit-content !important;
  padding-right: 5%;
  padding-left: 5%;
  grid-column: span 4;
  grid-row: span 1;
  margin-right: 10px !important;
  margin-left: auto !important;
}
.divider {
  margin: 0 100px;
  margin-top: 10px;
}
.loading {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -100%);
}
</style>
