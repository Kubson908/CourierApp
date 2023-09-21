import { Component } from "vue";

export type Route = {
  path: string;
  component: Component;
  meta: { roles: Array<string> | null };
  children?: Array<Route>;
};
