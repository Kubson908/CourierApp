export const sidelinks: {
  [key: string]: Array<link>;
} = {
  admin: [{ link: "/administration", title: "Administracja" }],
  dispatcher: [
    { link: "/shipments", title: "Przesyłki" },
    { link: "/couriers", title: "Kurierzy" },
  ],
};

type link = { link: string; title: string };
