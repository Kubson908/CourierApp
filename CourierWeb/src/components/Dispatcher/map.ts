import { Feature, Map, Overlay, View } from "ol";
import { Point } from "ol/geom";
import TileLayer from "ol/layer/Tile";
import VectorLayer from "ol/layer/Vector";
import { OSM } from "ol/source";
import VectorSource from "ol/source/Vector";
import { Icon, Style } from "ol/style";
import { LocalCoords, PopupInfo } from "../../typings";
import { Shipment } from "../../typings/shipment";
import { Control, FullScreen, defaults as defaultControls } from "ol/control";
import { ref } from "vue";
import axios from "axios";
import {
  openServiceApi,
  openServiceKey,
  warehouseCoordinates,
} from "../../config";

const center = ref([20.46, 53.76]);
const projection = ref("EPSG:4326");
const zoom = ref(13.5);
const rotation = ref(0);

export const title = ref<string>();
export const titleClass = ref<string>();
export const address = ref<string>();
export const city = ref<string>();
export const postalCode = ref<string>();
export const route = ref<Array<Shipment>>([]);

let popup: Overlay;

let container: HTMLElement | null;
let closer: HTMLElement | null;
let dropContainer: HTMLElement | null;

const isDragging = ref<boolean>(false);

let draggedElement: HTMLImageElement | undefined;

let map: Map;
let vectorLayer: VectorLayer<VectorSource>;
export const createMap = (
  localCoords: Array<LocalCoords>,
  shipments: Array<Shipment>
) => {
  map = new Map({
    controls: defaultControls().extend([
      new FullScreen({ source: "fullscreen" }),
    ]),
    target: "map",
    layers: [
      new TileLayer({
        source: new OSM(),
      }),
    ],
    view: new View({
      center: center.value,
      projection: projection.value,
      zoom: zoom.value,
      rotation: rotation.value,
    }),
  });
  vectorLayer = new VectorLayer({
    source: new VectorSource(),
    zIndex: 10,
  });
  map.addLayer(vectorLayer);

  localCoords?.forEach((localCoord) => {
    const multipleMarker: boolean =
      localCoords.filter(
        (c) =>
          c.coordinates[0] == localCoord.coordinates[0] &&
          c.coordinates[1] == localCoord.coordinates[1]
      ).length > 1;
    const marker = new Feature({
      geometry: new Point(localCoord.coordinates),
      name: localCoord.id,
      status: localCoord.status,
      multiple: multipleMarker,
    });
    marker.setId(localCoord.id);
    marker.setStyle(
      new Style({
        image: new Icon({
          src:
            marker.get("multiple") == true
              ? "/src/assets/multiple_packages.svg"
              : localCoord.status == 0
              ? "/src/assets/pickup.svg"
              : "/src/assets/delivery.svg",
          anchor: [0.5, 1],
        }),
        zIndex: 100,
      })
    );
    vectorLayer.getSource()!.addFeature(marker);
  });

  const control = new Control({
    element: document.getElementById("drop-container")!,
  });

  map.addControl(control);

  container = document.getElementById("popup")!;
  closer = document.getElementById("popup-closer")!;
  dropContainer = document.getElementById("drop-container");

  popup = new Overlay({
    element: container,
    positioning: "bottom-center",
    stopEvent: false,
    offset: [0, -50],
  });
  map.addOverlay(popup);

  addEventListeners(map, shipments);
};

export const popupList = ref<Array<PopupInfo>>([]);
let image: HTMLImageElement | null = null;
const addEventListeners = (map: Map, shipments: Array<Shipment>) => {
  // event kliknięcia w znacznik
  map.on("click", (event) => {
    if (map.hasFeatureAtPixel(event.pixel)) {
      popupList.value = [];
    }
    map.forEachFeatureAtPixel(event.pixel, (feature) => {
      isDragging.value = false;
      map.getInteractions().forEach((x) => x.setActive(true));
      event.stopPropagation();
      const name = feature.get("name") as number;
      const geometry = feature.getGeometry();
      if (geometry instanceof Point) {
        const coordinates = geometry.getCoordinates();
        popup.set("popupId", name);
        popup.getElement()!.hidden = false;
        var shipment = shipments?.find((s) => s.id == name);
        popupList.value.push({
          id: name,
          imgSrc:
            shipment?.status == 0
              ? "/src/assets/pickup.svg"
              : "/src/assets/delivery.svg",
          titleClass:
            shipment?.status == 0 ? "gray-text" : "pigment-green-text",
          title: shipment?.status == 0 ? "Odbiór" : "Dostawa",
          address:
            shipment?.status == 0
              ? shipment?.pickupAddress +
                (shipment?.pickupApartmentNumber
                  ? "/" + shipment?.pickupApartmentNumber
                  : "")
              : shipment?.recipientAddress +
                (shipment?.recipientApartmentNumber
                  ? "/" + shipment?.recipientApartmentNumber
                  : ""),
          city:
            shipment!.status == 0
              ? shipment!.pickupCity
              : shipment!.recipientCity,
          postalCode:
            shipment!.status == 0
              ? shipment!.pickupPostalCode
              : shipment!.recipientPostalCode,
        });
        popup.setPosition(coordinates);
      }
    });
  });
  // event poruszania kursorem => sprawdzamy czy kursor najechał na znacznik przesyłki
  map.on("pointermove", (event) => {
    const pixel = map.getEventPixel(event.originalEvent);
    const hit = map.hasFeatureAtPixel(pixel);
    map.getTargetElement().style.cursor = hit ? "pointer" : "";
  });

  map.getTargetElement().addEventListener("mousedown", (event) => {
    const pixel = map.getEventPixel(event);
    const target = event.target;
    if (target instanceof HTMLImageElement) {
      if (target.classList.contains("popup-img")) {
        image = target;
        isDragging.value = true;
        map.getInteractions().forEach((x) => x.setActive(false));
      }
    }
    map.forEachFeatureAtPixel(pixel, () => {
      isDragging.value = true;
      map.getInteractions().forEach((x) => x.setActive(false));
    });
  });

  map.getTargetElement().addEventListener("mousemove", (event) => {
    if (isDragging.value && !draggedElement) {
      let feature, status, id;
      if (image != null) {
        id = image.id;
        status = shipments.find((s) => s.id == id!)?.status;
      } else {
        feature = map.getFeaturesAtPixel(map.getEventPixel(event))[0];
        if (feature.get("multiple")) {
          isDragging.value = false;
          return;
        }
        status = feature.get("status");
        id = feature.get("name");
      }

      draggedElement = document.createElement("img");
      draggedElement.src =
        status == 0 ? "/src/assets/pickup.svg" : "/src/assets/delivery.svg";
      draggedElement.style.position = "fixed";
      draggedElement.style.height = "50px";
      draggedElement!.style.left = "-100px";
      draggedElement!.style.top = "-100px";
      draggedElement.id = id;
      document.getElementById("map")!.appendChild(draggedElement);
    } else if (isDragging.value) {
      draggedElement!.style.left = event.clientX - 25 + "px";
      draggedElement!.style.top = event.clientY - 25 + "px";

      const containerExtent = dropContainer?.getBoundingClientRect()!;
      const dropCoordinate = [event.clientX, event.clientY];

      if (
        dropCoordinate[0] >= containerExtent.left &&
        dropCoordinate[0] <= containerExtent.right &&
        dropCoordinate[1] >= containerExtent.top &&
        dropCoordinate[1] <= containerExtent.bottom &&
        isDragging.value
      ) {
        dropContainer!.style.boxShadow = "0px 0px 10px 10px #AFAFAF";
        dropContainer!.style.paddingRight = "30px";
      } else {
        dropContainer!.style.removeProperty("box-shadow");
        dropContainer!.style.removeProperty("padding-right");
      }
    }
  });

  map.getTargetElement().addEventListener("mouseup", (event) => {
    map.getInteractions().forEach((x) => x.setActive(true));
    if (draggedElement) {
      document.getElementById("map")!.removeChild(draggedElement);
      let id = draggedElement.id as unknown as number;
      draggedElement = undefined;
      image = null;
      const containerExtent = dropContainer?.getBoundingClientRect()!;
      const dropCoordinate = [event.clientX, event.clientY];
      if (
        dropCoordinate[0] >= containerExtent.left &&
        dropCoordinate[0] <= containerExtent.right &&
        dropCoordinate[1] >= containerExtent.top &&
        dropCoordinate[1] <= containerExtent.bottom &&
        isDragging.value
      ) {
        dropContainer!.style.removeProperty("box-shadow");
        dropContainer!.style.removeProperty("padding-right");
        if (route.value.find((s) => s.id == id) == null)
          route.value.push(shipments.find((s) => s.id == id) as Shipment);
        isDragging.value = false;
      } else {
        isDragging.value = false;
      }
    }
  });
};

// zamykanie popupu
export const closePopup = () => {
  popup.setPosition(undefined);
  closer?.blur();
  return false;
};

// usuwanie znaczników
export const removeFeatures = (featureIds: Array<number>) => {
  const source = vectorLayer.getSource();
  featureIds.forEach((element) => {
    source?.removeFeature(source.getFeatureById(element)!);
  });
};

const uniqueCoordinates = (
  coord: Array<number>,
  index: number,
  array: Array<Array<number>>
) => {
  return (
    array.findIndex((c) => c[0] === coord[0] && c[1] === coord[1]) === index
  );
};

// sortowanie trasy przy użyciu OpenRouteService Directions API
export const sortRoute = async (localCoords: Array<LocalCoords>) => {
  let coordinates: Array<Array<number>> = localCoords
    .filter((local) => {
      return route.value.some((e) => {
        return e.id == local.id && e.status === local.status;
      });
    })
    .map((local) => {
      return local.coordinates;
    });
  coordinates = coordinates.filter(uniqueCoordinates);
  coordinates.unshift(warehouseCoordinates);
  coordinates.push(warehouseCoordinates);

  const res = await axios.post(
    openServiceApi,
    {
      coordinates: coordinates,
      preference: "shortest",
    },
    {
      headers: {
        Authorization: openServiceKey,
      },
    }
  );
  const waypoints = res.data.features.map(
    (f: { properties: { way_points: any } }) => {
      return f.properties.way_points;
    }
  )[0];
  // console.log("waypoints: ", waypoints);
  const coords = res.data.features.map(
    (f: { geometry: { coordinates: any } }) => {
      return f.geometry.coordinates;
    }
  )[0];
  // console.log("coords: ", coords);

  const sortedCoords: Array<Array<number>> = [];
  waypoints.forEach((element: number) => {
    sortedCoords.push(coords[element]);
  });
  sortedCoords.shift();
  sortedCoords.pop();

  // console.log("sortedCoords: ", sortedCoords);

  const sortedRoute: Array<Shipment> = [];

  // console.log(localCoords);
  sortedCoords.forEach((element) => {
    const closest = findClosestOriginalCoords(element, localCoords);
    closest.forEach((c) => {
      const shipment = route.value.find((s) => s.id == c.id);
      if (shipment != null) sortedRoute.push(shipment!);
    });
  });

  console.log(sortedRoute);
  route.value = sortedRoute;
  // console.log(route.value);
};

// znalezienie najbliższych współrzędnych (kilka w przypadku wielu przesyłek pod tym samym adresem)
const findClosestOriginalCoords = (
  coord: Array<number>,
  localCoords: Array<LocalCoords>
): Array<LocalCoords> => {
  let previousDistance: number | null = null;
  let closestCoords: Array<LocalCoords> | null = null;
  localCoords.forEach((originalCoord) => {
    const distance = calculateDistance(coord, originalCoord.coordinates);
    if (previousDistance == null || distance < previousDistance) {
      previousDistance = distance;
      closestCoords = [originalCoord];
    } else if (previousDistance == null || distance == previousDistance) {
      closestCoords?.push(originalCoord);
    }
  });
  return closestCoords!;
};

const calculateDistance = (coord1: Array<number>, coord2: Array<number>) => {
  return Math.sqrt(
    Math.pow(coord1[0] - coord2[0], 2) + Math.pow(coord1[1] - coord2[1], 2)
  );
};
