<script setup lang="ts">
import { Feature, Map, Overlay, View } from "ol";
import { Point } from "ol/geom";
import TileLayer from "ol/layer/Tile";
import VectorLayer from "ol/layer/Vector";
import { OSM } from "ol/source";
import VectorSource from "ol/source/Vector";
import { Icon, Style } from "ol/style";
import { onMounted, ref } from "vue";
import { LocalCoords } from "../../typings";
import { Shipment } from "../../typings/shipment";

const emit = defineEmits(["closeMap"]);
const props = defineProps({
  localCoords: Array<LocalCoords>,
  shipments: Array<Shipment>,
});

const center = ref([20.46, 53.76]);
const projection = ref("EPSG:4326");
const zoom = ref(13.5);
const rotation = ref(0);

console.log(props.localCoords);

let map: Map;
let popup: Overlay;

let container: HTMLElement;
// let content: HTMLElement;
let closer: HTMLElement;

const title = ref<string>();
const titleClass = ref<string>();
const address = ref<string>();
const city = ref<string>();
const postalCode = ref<string>();
onMounted(() => {
  map = new Map({
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

  const vectorLayer = new VectorLayer({
    source: new VectorSource(),
  });
  map.addLayer(vectorLayer);
  props.localCoords?.forEach((localCoord) => {
    const marker = new Feature({
      geometry: new Point(localCoord.coordinates),
      name: localCoord.id,
    });
    marker.setStyle(
      new Style({
        image: new Icon({
          src:
            localCoord.status == 0
              ? "/src/assets/pickup.svg"
              : "/src/assets/delivery.svg",
          anchor: [0.5, 1],
        }),
      })
    );
    vectorLayer.getSource()!.addFeature(marker);
  });

  container = document.getElementById("popup")!;
  // content = document.getElementById("popup-content")!;
  closer = document.getElementById("popup-closer")!;

  popup = new Overlay({
    element: container,
    positioning: "bottom-center",
    stopEvent: false,
    offset: [0, -50],
  });
  map.addOverlay(popup);
  // event kliknięcia w znacznik
  map.on("click", (event) => {
    map.forEachFeatureAtPixel(event.pixel, (feature) => {
      const name = feature.get("name") as number;
      const geometry = feature.getGeometry();
      if (geometry instanceof Point) {
        const coordinates = geometry.getCoordinates();
        popup.set("popupId", name);
        popup.getElement()!.hidden = false;
        // content.innerHTML = props.shipments?.find(s => s.id == popup.get("popupId"));
        var shipment = props.shipments?.find((s) => s.id == name);
        titleClass.value =
          shipment?.status == 0 ? "grey-text" : "pigment-green-text";
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
  // event najechania kursorem na znacznik
  map.on("pointermove", (event) => {
    const pixel = map.getEventPixel(event.originalEvent);
    const hit = map.hasFeatureAtPixel(pixel);
    map.getTargetElement().style.cursor = hit ? "pointer" : "";
  });
});

const closePopup = () => {
  popup.setPosition(undefined);
  closer.blur();
  return false;
};
</script>

<template>
  <div class="fog" @click.self="emit('closeMap')">
    <div id="map" class="map"></div>
    <div id="popup">
      <button id="popup-closer" @click="closePopup()"></button>
      <div id="popup-content">
        <h2 :class="titleClass">{{ title }}</h2>
        <table>
          <tr>
            <td>Adres:</td>
            <td>{{ address }}</td>
          </tr>
          <tr>
            <td>Kod pocztowy:</td>
            <td>{{ postalCode }}</td>
          </tr>
          <tr>
            <td>Miasto:</td>
            <td>{{ city }}</td>
          </tr>
        </table>
      </div>
    </div>
  </div>
</template>

<style scoped>
.map {
  height: 80vh;
  width: 80vw;
  margin: auto;
  margin-top: 5%;
}

#popup {
  position: absolute;
  background-color: white;
  box-shadow: 0 1px 4px rgba(0, 0, 0, 0.6);
  padding: 15px;
  border-radius: 10px;
  border: 1px solid #cccccc;
  bottom: 12px;
  left: -50px;
  min-width: 280px;
  min-height: 100px;
}
#popup:after,
#popup:before {
  top: 100%;
  border: solid transparent;
  content: " ";
  height: 0;
  width: 0;
  position: absolute;
  pointer-events: none;
}
#popup:after {
  border-top-color: white;
  border-width: 10px;
  left: 48px;
  margin-left: -10px;
}
#popup:before {
  border-top-color: #cccccc;
  border-width: 11px;
  left: 48px;
  margin-left: -11px;
}
#popup-closer {
  text-decoration: none;
  position: absolute;
  top: 2px;
  right: 8px;
}
#popup-closer:after {
  content: "✖";
}
#popup-content {
  display: block;
}
td {
  text-align: start;
  padding: 0 5px;
}
</style>
