const prefix: string = "https://courier-app.azurewebsites.net"; //https://courier-app.azurewebsites.net | https://localhost:7119 | http://localhost:5274 | published local: https://localhost:5000
const geocodingKey: string = "AIzaSyDeqlvcFnk9C6KlHhw4G5NohIZyOAyucMg";
const googleApi: string = "https://maps.googleapis.com/maps/api/geocode/json";
const openServiceKey: string =
  "5b3ce3597851110001cf6248c3c76266e379499ab6666c42bd7af5e8";
const openServiceApi: string =
  "https://api.openrouteservice.org/v2/directions/driving-car/geojson";
const warehouseCoordinates: Array<number> = [
  20.456084839809897, 53.744143291594206,
];

const webSocketUrl = "wss://courier-app.azurewebsites.net"; //wss://courier-app.azurewebsites.net | wss://localhost:7119 | ws://localhost:5274
export {
  prefix,
  geocodingKey,
  googleApi,
  openServiceKey,
  openServiceApi,
  warehouseCoordinates,
  webSocketUrl,
};
