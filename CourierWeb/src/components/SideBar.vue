<script setup lang="ts">
import { ref } from "vue";
import { sidelinks } from "../structures/sidelinks";
import { user } from "../main";
import { useRoute } from "vue-router";

const hover = ref(false);

const links = user.roles.includes("Admin")
  ? sidelinks.admin
  : sidelinks.dispatcher;

const active: (path: string) => boolean = (path: string) => {
  if (useRoute().path == path) return true;
  return false;
};
</script>

<template>
  <ul
    @mouseover="hover = true"
    @mouseleave="hover = false"
    :class="{ active: hover }"
  >
    <li v-for="item in links">
      <div :class="hover ? 'visible' : 'hidden'">
        <a :href="item.link">
          {{ item.title }}
        </a>
      </div>
      <div :class="hover ? 'hidden' : 'visible'">
        <a
          :href="item.link"
          :class="active(item.link) ? 'activated' : 'inactive'"
          ><img :src="item.icon" class="icon" />
        </a>
      </div>
    </li>
    <!-- <li><a href="/">Home</a></li>
    <li><a href="/">Home</a></li>
    -->
  </ul>
</template>

<style scoped>
ul {
  list-style-type: none;
  margin: 0;
  padding: 0;
  overflow: hidden;
  background-color: #1b1919;
  height: 94.5vh;
  width: 3%;
  display: inline-block;
  transition: width 0.2s;
  font-size: 2vh;
}
li {
  height: 5.5vh;
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
li a:hover {
  background-color: #4d4d4d;
}
.active {
  width: 8%;
  transition: width 0.2s;
}
.visible {
  height: 100%;
  transition: transform 0.2s ease-in-out;
  z-index: 9;
}
.hidden {
  transform: translateX(-300%);
}
.icon {
  height: 100%;
  margin: 0;
}
.activated {
  background-color: #4d4d4d;
  transform: translateY(-37%);
}
.inactive {
  transform: translateY(-37%);
}
</style>
