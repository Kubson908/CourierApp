<script setup lang="ts">
import { user } from "../main";

const logout = () => {
  localStorage.clear();
  user.isLoggedIn = false;
  user.roles = [];
  user.name = "Niezalogowany";
};
</script>

<template>
  <ul>
    <li v-if="!user.roles.includes('Admin')"><a href="/">Home</a></li>
    <li v-if="!user.isLoggedIn" class="float-right">
      <a href="/login">Logowanie</a>
    </li>
    <li v-if="!user.isLoggedIn" class="float-right">
      <a href="/register">Rejestracja</a>
    </li>
    <li v-if="user.isLoggedIn" class="float-right">
      <a href="#" @click="logout">Wyloguj</a>
    </li>
  </ul>
</template>

<style scoped>
ul {
  list-style-type: none;
  margin: 0;
  padding: 0;
  overflow: hidden;
  background-color: #848c8e;
  width: 100%;
}

li {
  float: left;
}

li a {
  display: block;
  color: white;
  text-align: center;
  padding: 14px 16px;
  text-decoration: none;
}
li a:hover {
  background-color: #111;
}

.float-right {
  float: right;
}
</style>
