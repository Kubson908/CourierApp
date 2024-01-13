export const sidelinks: {
  [key: string]: Array<link>;
} = {
  admin: [
    {
      link: "/administration",
      title: "Pracownicy",
      icon: "people.svg",
    },
    {
      link: "/price-list",
      title: "Cennik",
      icon: "price-list.svg",
    },
  ],
  dispatcher: [
    { link: "/shipments", title: "Przesyłki", icon: "package.svg" },
    { link: "/couriers", title: "Kurierzy", icon: "people.svg" },
  ],
};

type link = { link: string; title: string; icon: string };
