<script setup lang="ts">
import { onBeforeMount, ref } from "vue";
import { authorized } from "../../main.ts";
import { Shipment } from "../../typings/shipment";
import { MapView } from ".";
import { manageCoordinates } from "../../geocoding";
import { LocalCoords } from "../../typings";

const showMap = ref<boolean>(false);
const shipments = ref<Array<Shipment>>();
const localCoords = ref<Array<LocalCoords>>([]);

onBeforeMount(async () => {
  const getShipments = await authorized.get(
    "/shipment/get-registered-shipments"
  );
  shipments.value = getShipments.data;
  await manageCoordinates(shipments.value!);
  const stringCoords = localStorage.getItem("localCoords");
  localCoords.value = stringCoords ? JSON.parse(stringCoords) : [];
});

const iconSource = (shipment: Shipment) => {
  return shipment.status == 0
    ? "/src/assets/pickup.svg"
    : "/src/assets/delivery.svg";
};

const size: Array<string> = ["Bardzo mały", "Mały", "Średni", "Duży"];
</script>

<template>
  <div id="adminDiv">
    <h1 class="pigment-green-text">Przesyłki do przypisania</h1>
    <div class="shipments-list">
      <div v-for="shipment in shipments">
        <img :src="iconSource(shipment)" />
        {{
          shipment.status == 0
            ? shipment.pickupAddress
            : shipment.recipientAddress
        }}
        {{ size[shipment.size!] }}
        <hr />
      </div>
      <button @click="showMap = true">Pokaż mapę</button>
      <MapView
        v-if="showMap"
        @closeMap="showMap = false"
        :localCoords="localCoords"
        :shipments="shipments"
      />
    </div>
  </div>
</template>

<style scoped>
.shipments-list {
  color: black;
  width: 80%;
  margin: auto;
}
</style>
