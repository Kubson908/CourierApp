<script setup lang="ts">
import { onBeforeMount, ref } from "vue";
import { authorized } from "../../main.ts";
import { Shipment } from "../../typings/shipment";
import { MapView, SubmitRoute } from ".";
import { manageCoordinates } from "../../geocoding";
import { Courier, LocalCoords } from "../../typings";
import { route, removeFeatures } from "./map";

const showMap = ref<boolean>(false);
const shipments = ref<Array<Shipment>>([]);
const localCoords = ref<Array<LocalCoords>>([]);
const couriers = ref<Array<Courier>>();
const unavailableDates = ref<{ [id: string]: Array<Date> }>({});
const selectedCourier = ref<Courier | null>(null);
const selectedDate = ref<string>();

onBeforeMount(async () => {
  let tomorrow = new Date();
  tomorrow.setDate(tomorrow.getDate() + 1);
  if (tomorrow.getDay() === 0) {
    tomorrow.setDate(tomorrow.getDate() + 1);
  }
  selectedDate.value = formatDate(tomorrow);

  const getShipments = await authorized.get(
    "/shipment/get-registered-shipments"
  );
  shipments.value = getShipments.data;

  const getUnavailableDates = await authorized.get(
    "/shipment/get-unavailable-dates"
  );
  unavailableDates.value = getUnavailableDates.data;

  await manageCoordinates(shipments.value!);
  const stringCoords = localStorage.getItem("localCoords");
  localCoords.value = stringCoords ? JSON.parse(stringCoords) : [];

  const getCouriers = await authorized.get("/admin/get-couriers");
  couriers.value = getCouriers.data;
});

const formatDate = (date: Date) => {
  const year = date.getFullYear();
  const month = ("0" + (date.getMonth() + 1)).slice(-2);
  const day = ("0" + date.getDate()).slice(-2);
  return `${year}-${month}-${day}`;
};

const validateDate = (event: Event) => {
  const date = new Date((event.target as HTMLInputElement).value);
  let tomorrow = new Date();
  tomorrow.setDate(tomorrow.getDate() + 1);
  if (tomorrow.getDay() === 0) {
    tomorrow.setDate(tomorrow.getDate() + 1);
  }
  if (date.getDay() === 0) {
    selectedDate.value = tomorrow.toISOString().split("T")[0];
    alert("Niedziela jest niedostępna. Wybierz inny dzień");
  } else if (date < new Date()) {
    selectedDate.value = tomorrow.toISOString().split("T")[0];

    alert("Nie można wybrać przeszłej daty");
  } else {
    selectedDate.value = (event.target as HTMLInputElement).value;
  }
};

const iconSource = (shipment: Shipment) => {
  return shipment.status == 0
    ? "/src/assets/pickup.svg"
    : "/src/assets/delivery.svg";
};

// const size: Array<string> = ["Bardzo mały", "Mały", "Średni", "Duży"];

const routesClearConfirmation = () => {
  if (route.value.length > 0) {
    if (confirm("Niezatwierdzone zmiany zostaną utracone")) {
      showMap.value = false;
      route.value = [];
    }
  } else showMap.value = false;
};

const confirmSubmit = ref<boolean>(false);

const submit = async () => {
  const res = await authorized.post("/shipment/set-route", {
    courierId: selectedCourier.value!.id,
    date: selectedDate.value,
    shipments: route.value,
  });
  if (res.status < 300) {
    confirmSubmit.value = false;
    removeFeatures(
      route.value.map((r) => {
        return r.id!;
      })
    );
    route.value = [];
  }
  console.log(res);
};
</script>

<template>
  <div>
    <h1 class="pigment-green-text">Przesyłki do przypisania</h1>
    <button @click="showMap = true" v-if="!showMap">Widok mapy</button>
    <button @click="routesClearConfirmation" v-else>Widok listy</button>
    <select
      v-model="selectedCourier"
      :class="selectedCourier == null ? 'gray' : 'white'"
    >
      <option value="null" selected hidden>Wybierz kuriera</option>
      <option v-for="courier in couriers" :key="courier.id" :value="courier">
        {{ courier.firstName + " " + courier.lastName }}
      </option>
    </select>
    <input type="date" v-model="selectedDate" @input="validateDate" />
    <div class="shipments-list" v-if="!showMap">
      <div v-for="shipment in shipments">
        <div class="list-element">
          <img :src="iconSource(shipment)" />
          {{
            shipment.status == 0
              ? shipment.pickupAddress
              : shipment.recipientAddress
          }}
          {{
            shipment.status == 0 ? shipment.pickupCity : shipment.recipientCity
          }}
        </div>
        <hr />
      </div>
    </div>
    <div v-if="showMap">
      <MapView
        @closeMap="showMap = false"
        :localCoords="localCoords"
        :shipments="shipments"
        :courier="selectedCourier"
        :date="selectedDate!"
        @submit="confirmSubmit = true"
      />
    </div>
    <SubmitRoute
      v-if="confirmSubmit"
      @cancelSubmit="confirmSubmit = false"
      @submit="submit"
    />
  </div>
</template>

<style scoped>
.shipments-list {
  color: black;
  width: 90%;
  margin: auto;
  margin-top: 10px;
}

.list-element {
  width: 20vw;
  margin: auto;
  text-align: start;
}
</style>
