<script setup lang="ts">
import { onBeforeMount, onBeforeUnmount, ref, watch } from "vue";
import { getCouriers, couriers } from "./couriers";
import { getWebSocket } from "./websocket";
import { user } from "../../main";
import { prefix } from "../../structures/config";
import { CourierInfo } from ".";
import { Courier } from "../../typings";

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

const replaceSrc = (e: any) => {
  e.target.src = "/src/assets/account-circle.svg";
};

const showInfo = ref<boolean>(false);
const selectedCourier = ref<Courier | null>(null);
const showModal = (courier: Courier) => {
  showInfo.value = true;
  selectedCourier.value = courier;
};
</script>

<template>
  <div>
    <h1 class="pigment-green-text">Kurierzy</h1>
    <div class="couriers-list">
      <div v-for="courier in couriers" class="card">
        <img
          :src="prefix + '/StaticFiles/' + courier.id + '.png'"
          @error="replaceSrc"
          class="profile-image"
        />
        <div class="content">
          <span
            style="font-weight: bold"
            :class="
              courier.status == 0
                ? 'pigment-green-text'
                : courier.status == 1
                ? 'yellow-text'
                : 'red-text'
            "
            >{{
              courier.status == 0
                ? "W trasie"
                : courier.status == 1
                ? "Poza zasięgiem"
                : "Nieaktywny"
            }}</span
          >
          <div class="item">
            <!-- <img :src="iconSource(shipment)" /> -->
            {{ courier.firstName + " " + courier.lastName }}
          </div>
          <button @click="showModal(courier)" class="info-button">Dane</button>
        </div>
      </div>
    </div>
    <teleport to="body">
      <CourierInfo
        v-if="showInfo"
        :courier="selectedCourier!"
        @close-modal="showInfo = false"
      />
    </teleport>
  </div>
</template>

<style scoped>
@media screen and (max-width: 1500px) {
  .couriers-list {
    color: black;
    display: grid;
    grid-template-columns: 1fr 1fr 1fr;
    width: 70%;
    margin: auto;
    margin-top: 10px;
  }
}

@media screen and (min-width: 1501px) {
  .couriers-list {
    color: black;
    display: grid;
    grid-template-columns: 1fr 1fr 1fr 1fr;
    width: 70%;
    margin: auto;
    margin-top: 10px;
  }
}

.card {
  background-color: white;
  width: 85%;
  min-height: 60%;
  border-radius: 12px;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.25);
  padding: 5% 4%;
  margin-bottom: 4vh;
  font-size: larger;
}
.content {
  width: 60%;
  float: right;
}
.item {
  height: 70px;
}
.profile-image {
  width: 40%;
  float: left;
  border-radius: 50%;
}
.info-button {
  width: fit-content !important;
}
</style>
../../structures/config
