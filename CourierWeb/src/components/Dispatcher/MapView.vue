<script setup lang="ts">
import {
  title,
  titleClass,
  city,
  address,
  postalCode,
  closePopup,
  routes,
} from "./map";
import { computed, onMounted } from "vue";
import { Courier, LocalCoords } from "../../typings";
import { Shipment } from "../../typings/shipment";
import { createMap } from "./map";
import { VueDraggableNext as draggable } from "vue-draggable-next";

const props = defineProps<{
  localCoords: Array<LocalCoords>;
  shipments: Array<Shipment>;
  courier: Courier | null;
  date: string;
}>();

onMounted(() => {
  createMap(props.localCoords!, props.shipments!);
});

const dragOptions = computed<{
  animation: 0;
  group: "description";
  disabled: false;
  ghostClass: "ghost";
}>;

const remove = (shipmentId: number) => {
  routes.value = routes.value.filter((s) => s.id != shipmentId);
};

const emit = defineEmits(["submit"]);
</script>

<template>
  <div id="fullscreen">
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
    <div
      id="drop-container"
      :class="
        courier ? ' blockdrop-container-active' : 'drop-container-inactive'
      "
    >
      <h3 class="pigment-green-text">
        Kurier {{ courier?.firstName + " " + courier?.lastName }}
      </h3>
      <div class="top">
        <draggable
          class="list-group"
          item-key="order"
          tag="transition-group"
          :component-data="{
            tag: 'div',
            name: 'shipment-route',
            type: 'transition',
          }"
          v-model="routes"
          v-bind="dragOptions"
        >
          <div v-for="shipment in routes" :key="shipment.id" class="draggable">
            <img
              :src="
                shipment.status == 0
                  ? '/src/assets/pickup.svg'
                  : '/src/assets/delivery.svg'
              "
            />
            <div class="flex">
              {{
                shipment.status == 0
                  ? shipment.pickupAddress
                  : shipment.recipientAddress
              }},
              {{
                shipment.status == 0
                  ? shipment.pickupCity
                  : shipment.recipientCity
              }}
            </div>
            <button @click="remove(shipment.id!)"></button>
          </div>
        </draggable>
      </div>
      <button v-if="routes.length >= 2" class="submit" @click="emit('submit')">
        Zatwierdź
      </button>
    </div>
  </div>
</template>

<style scoped>
.map {
  height: 100%;
  width: 100%;
  margin: auto;
  margin-top: 0;
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
  padding: 0;
}
#drop-container {
  display: flex;
  flex-direction: column;
  background: rgba(245, 245, 245, 0.69);
  min-height: 40%;
  float: right;
  margin: 40px -4px;
  border: 2px solid black;
  border-radius: 15px 0 0 15px;
  z-index: 1;
  transition: transform 0.3s ease-in-out, box-shadow 0.1s ease-in-out,
    padding-right 0.1s ease-in-out;
  padding: 0 20px 0 10px;
}
.drop-container-inactive {
  transform: translateX(100%);
  width: 0%;
}
.drop-container-active {
  width: 20%;
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
  color: black;
}
td {
  text-align: start;
  padding: 0 5px;
}
.draggable {
  background-color: white;
  color: black;

  margin-bottom: 5px;
  border: solid 1px black;
  border-radius: 3px;
  padding: 3px 0;
  display: flex;
}
.sortable-ghost {
  background-color: rgb(171, 195, 224);
}
.flex {
  display: flex;
  vertical-align: center;
  width: 85%;
  height: 100%;
  padding-left: 3px;
  font-size: 110%;
  line-height: 110%;
}

.draggable img {
  height: 25px;
  margin: 0 5px;
}

.draggable button {
  width: 30px;
  height: 25px;
  padding: 2px;
  float: right;
  margin: 0 5px;
  font-size: 100%;
  line-height: 100%;
}

.draggable button::after {
  content: "✖";
}

.bottom {
  height: 100%;
}
.top {
  min-height: 40%;
}
</style>
