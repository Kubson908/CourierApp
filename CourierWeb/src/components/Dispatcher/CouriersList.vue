<script setup lang="ts">
import { onBeforeMount, onBeforeUnmount, ref, watch } from "vue";
import { getCouriers, couriers } from "./couriers";
import { getWebSocket } from "./websocket";
import { user } from "../../main";

const socket = ref<WebSocket | null>(null);
const message = ref<any>(null);

onBeforeMount(async () => {
  getCouriers();
  if (user.roles.includes("Dispatcher")) socket.value = getWebSocket(message);
});

onBeforeUnmount(() => {
  try {
    socket.value?.close();
    socket.value = null;
  } catch (e: any) {
    console.log(e);
  }
});

watch(message, (newMessage) => {
  const parsed = JSON.parse(newMessage.data);
  if (parsed["EventName"] === "courierActive") {
    try {
      couriers.value.find((c) => c.id == parsed.Id)!.status = 0;
    } catch (error) {
      console.log(error);
      window.location.reload();
    }
  } else if (parsed["EventName"] === "recentlyActive") {
    try {
      couriers.value.find((c) => c.id == parsed.Id)!.status = 1;
    } catch (error) {
      console.log(error);
      window.location.reload();
    }
  } else if (parsed["EventName"] === "routeEnded") {
    try {
      couriers.value.find((c) => c.id == parsed.Id)!.status = 2;
    } catch (error) {
      console.log(error);
      window.location.reload();
    }
  }
});
</script>

<template>
  <div>
    <h1 class="pigment-green-text">Kurierzy</h1>
    <div class="couriers-list">
      <div v-for="courier in couriers" class="card">
        <div>
          <span
            :class="
              courier.status == 0
                ? 'pigment-green-text'
                : courier.status == 1
                ? 'yellow-text'
                : 'red-text'
            "
            >{{
              courier.status == 0
                ? " Aktywny"
                : courier.status == 1
                ? " Poza zasięgiem"
                : "Nieaktywny"
            }}</span
          >
          <div
            :class="
              courier.status == 0
                ? 'active-icon pigment-green'
                : courier.status == 1
                ? 'active-icon yellow'
                : 'active-icon red'
            "
          ></div>
        </div>

        <div class="list-element">
          <!-- <img :src="iconSource(shipment)" /> -->
          {{ courier.firstName + " " + courier.lastName }}
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.couriers-list {
  color: black;
  display: grid;
  grid-template-columns: 1fr 1fr 1fr 1fr;
  width: 70%;
  margin: auto;
  margin-top: 10px;
}

.card {
  background-color: white;
  width: 10vw;
  border-radius: 12px;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.25);
  padding: 5% 10%;
  margin-bottom: 4vh;
}

.active-icon {
  width: 1.5vh;
  height: 1.5vh;
  display: inline-flex;
  border-radius: 0.75vh;
  float: right;
}
</style>
