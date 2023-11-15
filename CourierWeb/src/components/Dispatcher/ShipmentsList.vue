<script setup lang="ts">
import { onBeforeMount, ref } from "vue";
import { authorized } from "../../main.ts";
import { Shipment } from "../../typings/shipment";
import { MapView } from ".";
import { manageCoordinates } from "../../geocoding";
import { Courier, LocalCoords } from "../../typings";

const showMap = ref<boolean>(false);
const shipments = ref<Array<Shipment>>([]);
const localCoords = ref<Array<LocalCoords>>([]);
const couriers = ref<Array<Courier>>();
const selectedCourier = ref<Courier | null>(null);

onBeforeMount(async () => {
  const getShipments = await authorized.get(
    "/shipment/get-registered-shipments"
  );
  shipments.value = getShipments.data;
  await manageCoordinates(shipments.value!);
  const stringCoords = localStorage.getItem("localCoords");
  localCoords.value = stringCoords ? JSON.parse(stringCoords) : [];

  const getCouriers = await authorized.get("/admin/get-couriers");
  couriers.value = getCouriers.data;
  console.log(couriers.value);
});

const iconSource = (shipment: Shipment) => {
  return shipment.status == 0
    ? "/src/assets/pickup.svg"
    : "/src/assets/delivery.svg";
};

const size: Array<string> = ["Bardzo mały", "Mały", "Średni", "Duży"];
</script>

<template>
  <div>
    <h1 class="pigment-green-text">Przesyłki do przypisania</h1>
    <button @click="showMap = true" v-if="!showMap">Widok mapy</button>
    <button @click="showMap = false" v-else>Widok listy</button>
    <select
      v-model="selectedCourier"
      :class="selectedCourier == null ? 'gray' : 'white'"
    >
      <option value="null" selected hidden>Wybierz kuriera</option>
      <option v-for="courier in couriers" :key="courier.id" :value="courier">
        {{ courier.firstName + " " + courier.lastName }}
      </option>
    </select>
    <div class="shipments-list" v-if="!showMap">
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
    </div>
    <div v-if="showMap">
      <MapView
        @closeMap="showMap = false"
        :localCoords="localCoords"
        :shipments="shipments"
        :courier="selectedCourier"
      />
    </div>
  </div>
</template>

<style scoped>
.shipments-list {
  color: black;
  width: 90%;
  margin: auto;
}
</style>
