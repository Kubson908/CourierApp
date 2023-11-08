<script setup lang="ts">
import { onBeforeMount, ref } from "vue";
import { authorized } from "../../main";
import { UserCard, EditUser, AddUser } from ".";

const dispatchers = ref<any>(null);

const fetchData = async () => {
  const getDispatchers = await authorized.get("/admin/get-dispatchers");
  dispatchers.value = getDispatchers.data.map((dispatcher: any) => {
    return {
      id: dispatcher.id,
      firstName: dispatcher.firstName,
      lastName: dispatcher.lastName,
      username: dispatcher.userName,
      email: dispatcher.email,
      phoneNumber: dispatcher.phoneNumber,
      role: "Dispatcher",
    };
  });
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
const showAdd = ref(false);
const add = () => (showAdd.value = true);
</script>

<template>
  <div id="adminDiv">
    <h1 color="#ff0000">Dyspozytorzy</h1>
    <UserCard
      v-for="dispatcher in dispatchers"
      :user="dispatcher"
      @edit="edit(dispatcher)"
    />
    <teleport to="body">
      <EditUser
        :user="selectedUser"
        v-if="showEdit"
        @closeModal="showEdit = false"
      />
      <AddUser
        v-if="showAdd"
        @closeModal="showAdd = false"
        @fetchData="fetchData"
      />
    </teleport>
    <br />
    <button id="addButton" @click="add">Dodaj</button>
  </div>
</template>

<style scoped>
#adminDiv {
  padding: 0 10%;
}
#addButton {
  float: right;
}
</style>
