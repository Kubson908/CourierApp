import { Route } from "./typings";
import { AdminPage, PriceList } from "./components/Admin";
import { HomePage, ProfilePage } from "./components";
import {
  LoginForm,
  RegisterForm,
  ResetPassword,
  VerifyEmail,
} from "./components/Guest";
import {
  DownloadLabels,
  OrderShipments,
  OrderArchive,
} from "./components/Customer";
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
  {
    path: "/administration",
    component: AdminPage,
    meta: { roles: ["Admin"] },
  },
  {
    path: "/price-list",
    component: PriceList,
    meta: { roles: ["Admin"] },
  },
  {
    path: "/login",
    component: LoginForm,
    meta: { roles: null },
    beforeEnter: () => {
      if (user.roles.length > 0) {
        return {
          path: "/",
        };
      }
    },
  },
  {
    path: "/register",
    component: RegisterForm,
    meta: { roles: null },
    beforeEnter: () => {
      if (user.roles.length > 0) {
        return {
          path: "/",
        };
      }
    },
  },
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
  {
    path: "/profile",
    component: ProfilePage,
    meta: { roles: ["Customer", "Dispatcher", "Admin"] },
  },
  { path: "/history", component: OrderArchive, meta: { roles: ["Customer"] } },
];
