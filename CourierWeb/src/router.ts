import { Route } from "./typings";
import { AdminPage } from "./components/Admin";
import { HomePage } from "./components";
import { LoginForm, RegisterForm } from "./components/Guest";
import { OrderShipments } from "./components/Customer";
import { ShipmentsList } from "./components/Dispatcher";

export const routes: Array<Route> = [
  { path: "/", component: HomePage, meta: { roles: null } },
  { path: "/administration", component: AdminPage, meta: { roles: ["Admin"] } },
  { path: "/login", component: LoginForm, meta: { roles: null } },
  { path: "/register", component: RegisterForm, meta: { roles: null } },
  { path: "/order", component: OrderShipments, meta: { roles: ["Customer"] } },
  { path: "/shipments", component: ShipmentsList, meta: { roles: ["Dispatcher"] } },
];
