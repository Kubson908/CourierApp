import { createApp, reactive, ref } from "vue";
import axios from "axios";
import { prefix } from "./config";
import "./style.css";
import App from "./App.vue";
import { User } from "./typings";
import * as VueRouter from "vue-router";
import { routes } from "./router";

export const loading = ref(false);

export const user = reactive<User>({
  name: localStorage.getItem("user") ?? "Niezalogowany",
  isLoggedIn: localStorage.getItem("user") ? true : false,
  roles: localStorage.getItem("roles")
    ? JSON.parse(localStorage.getItem("roles") ?? "")
    : [],
});

export const getToken = () => {
  return localStorage.getItem("token") ? localStorage.getItem("token") : null;
};

export const authorized = axios.create({
  baseURL: `${prefix}/api`,
  timeout: 10000,
});
authorized.interceptors.request.use((config) => {
  config.headers.Authorization = `Bearer ${localStorage.getItem("token")}`;
  return config;
});

export const unauthorized = axios.create({
  baseURL: `${prefix}/api`,
  timeout: 10000,
});

declare module "vue-router" {
  interface RouteMeta {
    roles: Array<string> | null;
  }
}
export const router = VueRouter.createRouter({
  history: VueRouter.createWebHistory(),
  routes,
});

router.beforeEach((to, from) => {
  if (
    to.meta.roles != null &&
    !to.meta.roles.some((e) => user.roles.includes(e))
  ) {
    return {
      path: "/",
      query: { redirect: from.fullPath },
    };
  }
});

createApp(App).use(router).mount("#app");
