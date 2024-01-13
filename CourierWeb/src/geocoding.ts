import axios from "axios";
import { Shipment } from "./typings/shipment";
import { LocalCoords } from "./typings";
import { geocodingKey, googleApi } from "./structures/config";

type AddressInfo = {
  id: number;
  status: number;
  address: string;
};

export const manageCoordinates = async (shipments: Array<Shipment>) => {
  let localCoords: Array<LocalCoords> = JSON.parse(
    localStorage.getItem("localCoords") ?? "[]"
  ) as Array<LocalCoords>;
  const oldShipments = localCoords.filter((local) => {
    return shipments.some((shipment) => {
      return shipment.id === local.id && shipment.status === local.status;
    });
  });
  localCoords = [...oldShipments];

  const newShipments = shipments
    .filter((shipment) => {
      return !localCoords.some((local) => {
        return shipment.id === local.id && shipment.status === local.status;
      });
    })
    .map((s: Shipment): AddressInfo => {
      if (s.status == 0 || s.status == 7)
        return {
          id: s.id!,
          status: s.status,
          address:
            s.pickupAddress +
            " " +
            s.pickupCity +
            " " +
            s.pickupPostalCode +
            " Poland",
        };
      else
        return {
          id: s.id!,
          status: s.status!,
          address:
            s.recipientAddress +
            " " +
            s.recipientCity +
            " " +
            s.recipientPostalCode +
            " Poland",
        };
    });
  if (newShipments.length > 0) {
    const newCoords: Array<LocalCoords> = await getCoordinates(newShipments);
    localCoords.push(...newCoords);
  }
  localStorage.setItem("localCoords", JSON.stringify(localCoords));
};

const getCoordinates = async (addresses: Array<AddressInfo>) => {
  let coords: Array<LocalCoords> = [];
  for (const address of addresses) {
    const res = await axios.get(googleApi, {
      params: {
        address: address.address,
        key: geocodingKey,
      },
    });
    coords.push({
      id: address.id,
      status: address.status,
      coordinates: [
        res.data.results[0].geometry.location.lng,
        res.data.results[0].geometry.location.lat,
      ],
    });
  }
  return coords;
};
