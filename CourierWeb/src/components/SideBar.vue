<script setup lang="ts">
import { ref } from "vue";

const hover = ref(false);
</script>

<template>
  <ul
    @mouseover="hover = true"
    @mouseleave="hover = false"
    :class="{ active: hover }"
  >
    <li><a href="/">Home</a></li>
    <li><a href="/">Home</a></li>
  </ul>
</template>

<style scoped>
ul {
  list-style-type: none;
  margin: 0;
  padding: 0;
  overflow: hidden;
  background-color: #1b1919;
  height: 100%;
  width: 3%;
  display: inline-block;
  transition: width 0.2s;
}

li a {
  display: block;
  color: white;
  text-align: center;
  padding: 14px 0;
  text-decoration: none;
}
li a:hover {
  background-color: #111;
}
.active {
  width: 8%;
  transition: width 0.2s;
}
</style>
