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

const loading = ref<boolean>(false);

const getShipmentsList = async () => {
  const getShipments = await authorized.get(
    "/shipment/get-registered-shipments"
  );
  shipments.value = getShipments.data;
};

onBeforeMount(async () => {
  loading.value = true;
  let tomorrow = new Date();
  tomorrow.setDate(tomorrow.getDate() + 1);
  if (tomorrow.getDay() === 0) {
    tomorrow.setDate(tomorrow.getDate() + 1);
  }
  selectedDate.value = formatDate(tomorrow);
  try {
    await getShipmentsList();

    const getUnavailableDates = await authorized.get(
      "/shipment/get-unavailable-dates"
    );
    unavailableDates.value = getUnavailableDates.data;
    await manageCoordinates(shipments.value!);
    const stringCoords = localStorage.getItem("localCoords");
    localCoords.value = stringCoords ? JSON.parse(stringCoords) : [];

    const getCouriers = await authorized.get("/admin/get-couriers");
    couriers.value = getCouriers.data;
  } catch (error: any) {
  } finally {
    loading.value = false;
  }
});

const formatDate = (date: Date) => {
  const year = date.getFullYear();
  const month = ("0" + (date.getMonth() + 1)).slice(-2);
  const day = ("0" + date.getDate()).slice(-2);
  return `${year}-${month}-${day}`;
};
// TODO: dodac jakis lepszy date picker i blokować w nim zajęte daty przy zmianie kuriera
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
    ? "pickup.svg"
    : shipment.status == 3
    ? "delivery.svg"
    : "return.svg";
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
  try {
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
      route.value.forEach((route) => {
        localCoords.value = localCoords.value.filter((l) => l.id != route.id);
      });
      route.value = [];
    }
    await getShipmentsList();
  } catch {
    alert("Nie udało się zapisać trasy");
  }
};
</script>

<template>
  <div class="card">
    <div class="vh20">
      <h1 class="pigment-green-text">Przesyłki do przypisania</h1>
      <button
        @click="showMap = true"
        v-if="!showMap"
        class="pigment-green submit const-width"
      >
        Widok mapy
      </button>
      <button
        @click="routesClearConfirmation"
        v-else
        class="pigment-green submit const-width"
      >
        Widok listy
      </button>
      <select
        v-model="selectedCourier"
        :class="
          selectedCourier == null ? 'gray rounded-input' : 'white rounded-input'
        "
        style="height: 34px"
      >
        <option value="null" selected hidden>Wybierz kuriera</option>
        <option v-for="courier in couriers" :key="courier.id" :value="courier">
          {{ courier.firstName + " " + courier.lastName }}
        </option>
      </select>
      <input
        type="date"
        v-model="selectedDate"
        @input="validateDate"
        class="rounded-input"
        onkeydown="return false"
      />
    </div>
    <h2 class="black-text" v-if="shipments.length == 0 && !loading && !showMap">
      Brak nowych przesyłek
    </h2>
    <div class="shipments-list" v-if="!showMap">
      <div v-for="shipment in shipments">
        <div class="list-element">
          <img :src="iconSource(shipment)" />
          {{
            shipment.status == 0 || shipment.status == 7
              ? shipment.pickupAddress
              : shipment.recipientAddress
          }}
          {{
            shipment.status == 0 || shipment.status == 7
              ? shipment.pickupCity
              : shipment.recipientCity
          }}
        </div>
        <hr v-if="shipments[shipments.length - 1] != shipment" />
      </div>
      <div v-if="loading && shipments.length == 0">
        <img src="/src/assets/loading.gif" class="loading" />
      </div>
    </div>
    <div v-else-if="!loading" class="map-container">
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
.map-container {
  height: 64vh;
}
.vh20 {
  height: 20vh;
  margin: 0 !important;
}
.card {
  background-color: white;
  width: 60%;
  height: fit-content;
  margin-top: 12vh;
  margin-bottom: 0;
  margin-left: auto;
  margin-right: auto;
  border-radius: 12px;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.25);
  padding: 0 !important;
}
.shipments-list {
  color: black;
  margin: 0 auto;
  width: 90%;
  height: 63vh;
  margin-top: 10px;
  overflow-y: auto;
}
.shipments-list::-webkit-scrollbar {
  width: 10px;
}
.shipments-list::-webkit-scrollbar-track {
  border-radius: 10px;
  background: #bdbdbd;
}
.shipments-list::-webkit-scrollbar-thumb {
  background: #15ab54;
  border-radius: 10px;
}
.shipments-list::-webkit-scrollbar-thumb:hover {
  background: #129448;
}
.list-element {
  width: 20vw;
  margin: auto;
}
.const-width {
  width: 20% !important;
  margin: 10px 5px 10px 5px !important;
}
.rounded-input {
  border-radius: 10px;
  height: 30px;
  width: 20%;
  margin: 5px;
  background-color: #f6f6f6;
  border: solid 2px #e8e8e8;
  color: black;
  text-align: center;
}
input[type="date"]::-webkit-calendar-picker-indicator {
  filter: invert(1);
  width: 100%;
  position: absolute;
  padding-left: 11%;
}
</style>
