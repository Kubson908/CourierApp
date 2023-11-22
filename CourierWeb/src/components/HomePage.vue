<script setup lang="ts">
import { user } from '../main';
</script>
<template>
  <h1 class="pigment-green-text">HOME PAGE</h1>
  <a href="/order" v-if="user.roles.includes('Customer')">Zamów kuriera</a>
</template>
<style scoped></style>
