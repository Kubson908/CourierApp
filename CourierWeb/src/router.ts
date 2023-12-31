import { Route } from "./typings";
import { AdminPage } from "./components/Admin";
import { HomePage } from "./components";
import {
  LoginForm,
  RegisterForm,
  ResetPassword,
  VerifyEmail,
} from "./components/Guest";
import { DownloadLabels, OrderShipments } from "./components/Customer";
import { MapView, ShipmentsList, CouriersList } from "./components/Dispatcher";
import { user } from "./main";

export const routes: Array<Route> = [
  {
    path: "/",
    component: HomePage,
    meta: { roles: null },
    beforeEnter: () => {
      if (user.roles.includes("Admin")) {
        return {
          path: "/administration",
        };
      }
    },
  },
  { path: "/administration", component: AdminPage, meta: { roles: ["Admin"] } },
  { path: "/login", component: LoginForm, meta: { roles: null } },
  { path: "/register", component: RegisterForm, meta: { roles: null } },
  {
    path: "/order",
    component: OrderShipments,
    meta: { roles: ["Customer"] },
  },
  {
    path: "/order-registered",
    component: DownloadLabels,
    meta: { roles: ["Customer"] },
  },
  {
    path: "/shipments",
    component: ShipmentsList,
    meta: { roles: ["Dispatcher"] },
  },
  { path: "/map", component: MapView, meta: { roles: ["Dispatcher"] } },
  {
    path: "/couriers",
    component: CouriersList,
    meta: { roles: ["Dispatcher"] },
  },
  { path: "/verify", component: VerifyEmail, meta: { roles: null } },
  { path: "/reset-password", component: ResetPassword, meta: { roles: null } },
];
