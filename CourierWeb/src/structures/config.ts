const prefix: string = "https://localhost:7119";
const geocodingKey: string = "AIzaSyDeqlvcFnk9C6KlHhw4G5NohIZyOAyucMg";
const googleApi: string = "https://maps.googleapis.com/maps/api/geocode/json";
const openServiceKey: string =
  "5b3ce3597851110001cf6248c3c76266e379499ab6666c42bd7af5e8";
const openServiceApi: string =
  "https://api.openrouteservice.org/v2/directions/driving-car/geojson";
const warehouseCoordinates: Array<number> = [
  20.456084839809897, 53.744143291594206,
];

const webSocketUrl = "wss://localhost:7119";
export {
  prefix,
  geocodingKey,
  googleApi,
  openServiceKey,
  openServiceApi,
  warehouseCoordinates,
  webSocketUrl,
};
