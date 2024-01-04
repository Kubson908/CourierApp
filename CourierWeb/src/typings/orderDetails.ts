import { Shipment } from "./shipment";

export type OrderDetails = {
  orderDate: string;
  shipments: Array<Shipment>;
};
