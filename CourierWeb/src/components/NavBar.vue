<script setup lang="ts">
import { router, user } from "../main";

const logout = () => {
  localStorage.clear();
  user.isLoggedIn = false;
  user.roles = [];
  user.name = "Niezalogowany";
};
</script>

<template>
  <ul>
    <li v-if="!user.roles.includes('Admin')"><a href="/">Strona główna</a></li>
    <li v-if="!user.isLoggedIn" class="float-right">
      <a href="/login">Logowanie</a>
    </li>
    <li v-if="!user.isLoggedIn" class="float-right">
      <a href="/register">Rejestracja</a>
    </li>
    <li v-if="user.isLoggedIn" class="float-right">
      <a href="#" @click="logout">Wyloguj</a>
    </li>
    <li v-if="user.isLoggedIn" class="float-right">
      <img
        src="/src/assets/account-circle-white.svg"
        class="profile-icon"
        @click="router.push('/profile')"
      />
    </li>
    <div v-if="user.isLoggedIn" class="float-right username">
      <a class="username">{{ user.name }}</a>
    </div>
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
  height: 5.5vh;
  font-size: 2vh;
}

li {
  float: left;
  height: 100%;
  width: fit-content;
}

li a {
  display: flex;
  padding: 0 1vw;
  color: white;
  line-height: 100%;
  height: 100%;
  justify-content: center;
  align-items: center;
  text-decoration: none;
}
li a:hover,
.profile-icon:hover {
  background-color: #111;
}

.float-right {
  float: right;
}
.profile-icon {
  height: 100%;
  cursor: pointer;
}
.username {
  display: flex;
  padding: 0 0.5vw;
  color: white;
  line-height: 100%;
  height: 100%;
  justify-content: center;
  align-items: center;
  text-decoration: none;
  font-style: normal !important;
}
</style>
