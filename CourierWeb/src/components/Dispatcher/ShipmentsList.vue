<script setup lang="ts">
import { onBeforeMount, ref } from "vue";
import { authorized } from "../../main.ts";
import { Shipment } from "../../typings/shipment";

const shipments = ref<Array<Shipment>>();

onBeforeMount(async () => {
  const getShipments = await authorized.get(
    "/shipment/get-registered-shipments"
  );
  shipments.value = getShipments.data;
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
