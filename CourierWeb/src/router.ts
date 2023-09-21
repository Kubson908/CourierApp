import { Route } from "./typings";

import { HomePage, AdminPage, LoginForm } from "./components";

export const routes: Array<Route> = [
  { path: "/", component: HomePage, meta: { roles: null } },
  { path: "/administration", component: AdminPage, meta: { roles: ["Admin"] } },
  { path: "/login", component: LoginForm, meta: { roles: null } },
];
