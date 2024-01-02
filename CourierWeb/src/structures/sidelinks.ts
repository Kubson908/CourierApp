export const sidelinks: {
  [key: string]: Array<link>;
} = {
  admin: [
    {
      link: "/administration",
      title: "Pracownicy",
      icon: "/src/assets/people.svg",
    },
    {
      link: "/price-list",
      title: "Cennik",
      icon: "/src/assets/package.svg",
    },
  ],
  dispatcher: [
    { link: "/shipments", title: "Przesyłki", icon: "/src/assets/package.svg" },
    { link: "/couriers", title: "Kurierzy", icon: "/src/assets/people.svg" },
  ],
};

type link = { link: string; title: string; icon: string };
