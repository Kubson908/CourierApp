import { Feature, Map, Overlay, View } from "ol";
import { Point } from "ol/geom";
import TileLayer from "ol/layer/Tile";
import VectorLayer from "ol/layer/Vector";
import { OSM } from "ol/source";
import VectorSource from "ol/source/Vector";
import { Icon, Style } from "ol/style";
import { LocalCoords } from "../../typings";
import { Shipment } from "../../typings/shipment";
import { Control, FullScreen, defaults as defaultControls } from "ol/control";
import { ref } from "vue";

const center = ref([20.46, 53.76]);
const projection = ref("EPSG:4326");
const zoom = ref(13.5);
const rotation = ref(0);

export const title = ref<string>();
export const titleClass = ref<string>();
export const address = ref<string>();
export const city = ref<string>();
export const postalCode = ref<string>();
export const routes = ref<Array<Shipment>>([]);

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
    const marker = new Feature({
      geometry: new Point(localCoord.coordinates),
      name: localCoord.id,
      status: localCoord.status,
    });
    marker.setId(localCoord.id);
    marker.setStyle(
      new Style({
        image: new Icon({
          src:
            localCoord.status == 0
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

const addEventListeners = (map: Map, shipments: Array<Shipment>) => {
  // event kliknięcia w znacznik
  map.on("click", (event) => {
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
        titleClass.value =
          shipment?.status == 0 ? "gray-text" : "pigment-green-text";
        title.value = shipment?.status == 0 ? "Odbiór" : "Dostawa";
        address.value =
          shipment?.status == 0
            ? shipment?.pickupAddress +
              (shipment?.pickupApartmentNumber
                ? "/" + shipment?.pickupApartmentNumber
                : "")
            : shipment?.recipientAddress +
              (shipment?.recipientApartmentNumber
                ? "/" + shipment?.recipientApartmentNumber
                : "");
        city.value =
          shipment?.status == 0
            ? shipment?.pickupCity
            : shipment?.recipientCity;
        postalCode.value =
          shipment?.status == 0
            ? shipment?.pickupPostalCode
            : shipment?.recipientPostalCode;
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
    map.forEachFeatureAtPixel(pixel, () => {
      isDragging.value = true;
      map.getInteractions().forEach((x) => x.setActive(false));
    });
  });

  map.getTargetElement().addEventListener("mousemove", (event) => {
    if (isDragging.value && !draggedElement) {
      let feature = map.getFeaturesAtPixel(map.getEventPixel(event))[0];
      let status = feature.get("status");
      draggedElement = document.createElement("img");
      draggedElement.src =
        status == 0 ? "/src/assets/pickup.svg" : "/src/assets/delivery.svg";
      draggedElement.style.position = "fixed";
      draggedElement.style.height = "50px";
      draggedElement!.style.left = "-100px";
      draggedElement!.style.top = "-100px";
      draggedElement.id = feature.get("name");
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
    if (draggedElement) {
      map.getInteractions().forEach((x) => x.setActive(true));
      document.getElementById("map")!.removeChild(draggedElement);
      let id = draggedElement.id as unknown as number;
      draggedElement = undefined;
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
        if (routes.value.find((s) => s.id == id) == null)
          routes.value.push(shipments.find((s) => s.id == id) as Shipment);
        isDragging.value = false;
      } else {
        isDragging.value = false;
      }
    }
  });
};

export const closePopup = () => {
  popup.setPosition(undefined);
  closer?.blur();
  return false;
};

export const removeFeatures = (featureIds: Array<number>) => {
  const source = vectorLayer.getSource();
  featureIds.forEach((element) => {
    console.log(source?.getFeatureById(element)!);
    source?.removeFeature(source.getFeatureById(element)!);
  });
};
