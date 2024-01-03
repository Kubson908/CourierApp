<script setup lang="ts">
import { closePopup, route, createMap, sortRoute, popupList } from "./map";
import { computed, onMounted } from "vue";
import { Courier, LocalCoords } from "../../typings";
import { Shipment } from "../../typings/shipment";
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
  route.value = route.value.filter((s) => s.id != shipmentId);
};

const sort = async () => {
  await sortRoute(props.localCoords);
};

const emit = defineEmits(["submit"]);
</script>

<template>
  <div id="fullscreen">
    <div id="map" class="map"></div>
    <div id="popup">
      <button id="popup-closer" @click="closePopup()"></button>
      <div id="popup-content" v-for="info in popupList">
        <img
          :src="info.imgSrc"
          class="popup-img"
          :id="info.id.toString()"
          draggable="false"
        />
        <h2 :class="info.titleClass">{{ info.title }}</h2>
        <table>
          <tr>
            <td>Adres:</td>
            <td>{{ info.address }}</td>
          </tr>
          <tr>
            <td>Kod pocztowy:</td>
            <td>{{ info.postalCode }}</td>
          </tr>
          <tr>
            <td>Miasto:</td>
            <td>{{ info.city }}</td>
          </tr>
        </table>
        <hr v-if="popupList[popupList.length - 1] != info" />
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
          v-model="route"
          v-bind="dragOptions"
        >
          <div v-for="shipment in route" :key="shipment.id" class="draggable">
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
                  ? shipment.pickupApartmentNumber
                    ? shipment.pickupAddress +
                      "/" +
                      shipment.pickupApartmentNumber
                    : shipment.pickupAddress
                  : shipment.recipientApartmentNumber
                  ? shipment.recipientAddress +
                    "/" +
                    shipment.recipientApartmentNumber
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
      <button v-if="route.length >= 2" class="submit" @click="emit('submit')">
        Zatwierdź
      </button>
      <button v-if="route.length >= 2" class="submit" @click="sort">
        Sortuj
      </button>
    </div>
  </div>
</template>

<style scoped>
.map {
  height: 100%;
  width: 100%;
  border-radius: 12px;
  overflow: hidden;
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
  height: 64vh;
  margin: 0 !;
  padding: 0 !;
  border-radius: 30px;
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
  padding: 0 !important;
  top: 0;
  right: 0;
  width: 30px !important;
  height: 30px !important;
  background-color: black !important;
  color: white !important;
  border-radius: 20% !important;
  justify-content: center !important;
  align-items: center !important;
  display: flex !important;
  font-size: 20px !important;
  line-height: 20px !important;
  text-align: center !important;
}
#popup-closer::after {
  content: "✖";
}
.popup-img {
  float: left;
  position: absolute;
  left: 5%;
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
