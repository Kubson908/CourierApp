<script setup lang="ts">
import { Map } from "ol";
import { title, titleClass, city, address, postalCode, closePopup } from "./map";
import { onMounted } from "vue";
import { LocalCoords } from "../../typings";
import { Shipment } from "../../typings/shipment";
import { createMap } from "./map";

const emit = defineEmits(["closeMap"]);
const props = defineProps({
  localCoords: Array<LocalCoords>,
  shipments: Array<Shipment>,
});

let map: Map;

onMounted(() => {
  map = createMap(props.localCoords!, props.shipments!);
});

</script>

<template>
  <div class="fog" @click.self="emit('closeMap')">
    <div id="fullscreen">
      <div id="map" class="map" ></div>
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
      <div id="drop-container"></div>
    </div>
  </div>
</template>

<style scoped>
.map {
  height: 100%;
  width: 100%;
  margin: auto;
}
.map:fullscreen,
.map:-webkit-full-screen {
  height: 100%;
  width: 100%;
}
#fullscreen:-webkit-full-screen {
  height: 100%;
  margin: 0;
  padding: 0;
}
#fullscreen:fullscreen {
  height: 100%;
  margin: 0;
  padding: 0;
}
#fullscreen {
  height: 80vh;
  width: 80vw;
  margin: auto;
  margin-top: 5%;
  padding: 0;
}
#drop-container {
  background: rgba(245, 245, 245, 0.69);
  width: 20%;
  height: 40%;
  float: right;
  margin: 40px -4px;
  border: 2px solid black;
  border-radius: 15px 0 0 15px;
  z-index: 1;
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
