<script setup lang="ts">
import { onBeforeMount, ref } from "vue";
import { RepeatOrderConfirmation } from ".";
import { OrderDetails } from "../../typings";
import { authorized, router } from "../../main";
import { useRoute } from "vue-router";
import { format } from "date-fns";
import { pl } from "date-fns/locale";

const orderDetails = ref<OrderDetails | null>(null);
const loading = ref<boolean>(false);
const expandList = ref<Array<boolean>>([]);

onBeforeMount(async () => {
  loading.value = true;
  const id = useRoute().query.id?.valueOf();
  try {
    const res = await authorized.get("/shipment/get-order-details/" + id);
    if (res.status == 200) {
      orderDetails.value = res.data;
      orderDetails.value?.shipments.forEach(() => {
        expandList.value.push(false);
      });
    }
  } catch (error: any) {}
  loading.value = false;
});

const formatDate = (date: string) => {
  return format(new Date(date), "EEEE, dd.MM.yyyy, HH:mm", { locale: pl });
};

const sizes: Array<string> = ["bardzo mały", "mały", "średni", "duży"];
const weights: Array<string> = [
  "lekka (do 5 kg)",
  "Średnia (10 - 15 kg)",
  "ciężka (powyżej 15 kg)",
];
const status: Array<string> = [
  "Zarejestrowana",
  "Zatwierdzona",
  "Podjęta",
  "Magazynowana",
  "W trakcie doręczenia",
  "Doręczona",
  "Nie zastano odbiorcy",
  "Do zwrotu",
  "W trakcie zwrotu",
  "Zwrócona do nadawcy",
];

const expand = (index: number) => {
  expandList.value[index] = !expandList.value[index];
  console.log(expandList.value);
};

const confirm = ref<boolean>(false);

const repeatOrder = async () => {
  confirm.value = false;
  loading.value = true;
  try {
    const res = await authorized.post("/shipment/repeat-order", {
      shipments: orderDetails.value?.shipments,
    });
    const shipmentsIds = res.data;
    if (res.status < 300) {
      router.push({ path: "/order-registered", query: { id: shipmentsIds } });
    }
  } catch {}
  loading.value = false;
};
</script>

<template>
  <div class="black-text background">
    <div v-if="!loading">
      <div v-if="orderDetails">
        <h1 class="pigment-green-text">Szczegóły zamówienia</h1>
        <div class="date">{{ formatDate(orderDetails!.orderDate) }}</div>
        <div class="list">
          <div v-for="(shipment, index) in orderDetails.shipments">
            <div class="info">
              <div class="list-element">
                <span class="col-1"
                  >{{ shipment.recipientCity }}
                  {{
                    shipment.recipientAddress +
                    (shipment.recipientApartmentNumber
                      ? "/" + shipment.recipientApartmentNumber
                      : "")
                  }}</span
                >
                <span class="col-2">Rozmiar: {{ sizes[shipment.size!] }}</span>
                <span class="col-3">Waga: {{ weights[shipment.weight!] }}</span>
                <span class="col-4">
                  <img
                    src="/src/assets/arrow.svg"
                    :class="expandList[index] ? 'expander expand' : 'expander'"
                    @click="expand(index)"
                  />
                </span>
              </div>
              <div
                :class="expandList[index] ? 'show-info more-info' : 'more-info'"
              >
                <div class="flex">
                  <span class="col-1"
                    >Odbiorca: {{ shipment.recipientName }}</span
                  >
                  <span class="col-2"
                    >Tel: {{ shipment.recipientPhoneNumber }}</span
                  >
                  <span class="col-3"
                    >E-mail: {{ shipment.recipientEmail }}</span
                  >
                  <span class="col-4"></span>
                </div>
                <div class="flex">
                  <span class="col-1">Cena: {{ shipment.price }} zł</span>
                  <span class="col-2"
                    >Status: {{ status[shipment.status!] }}</span
                  >
                  <span class="col-3"
                    >Prób doręczenia: {{ shipment.deliveryAttempts }}</span
                  >
                  <span class="col-4"></span>
                </div>
              </div>
            </div>
            <hr
              v-if="
                orderDetails.shipments[orderDetails.shipments.length - 1] !=
                shipment
              "
              class="divider"
            />
          </div>
        </div>
        <button class="submit pigment-green" @click="confirm = true">
          Powtórz zamówienie
        </button>
        <RepeatOrderConfirmation
          v-if="confirm"
          @cancel="confirm = false"
          @confirm="repeatOrder"
        />
      </div>
      <div v-else>
        <h1 class="red-text mt-10">Błąd pobierania danych</h1>
      </div>
    </div>
    <div v-else>
      <img src="/src/assets/loading.gif" class="loading" />
    </div>
  </div>
</template>

<style scoped>
.background {
  background-color: white;
  width: 60vw;
  margin: auto;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.25);
  flex-direction: column;
  display: flex;
  min-height: 94.5vh;
  padding: 0;
}
.loading {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
}
.date {
  font-size: 20px;
}
.list {
  margin-top: 10%;
}
.info {
  background-color: #f1f1f1;
}
.list-element {
  margin-left: 20px;
  margin-right: 20px;
  display: flex;
  height: 50px;
  font-size: 20px;
}
.divider {
  margin-left: 20px;
  margin-right: 20px;
}
.col-1 {
  width: 30%;
  align-self: center;
  text-align: start;
  padding-left: 10px;
}
.col-2 {
  width: 20%;
  align-self: center;
  text-align: start;
  padding-left: 10px;
}
.col-3 {
  width: 45%;
  align-self: center;
  text-align: start;
  padding-left: 10px;
}
.col-4 {
  width: 5%;
  align-self: center;
  display: flex;
}
.expander {
  height: 50px;
}
.expand {
  transform: rotate(90deg);
}
.flex {
  display: flex;
}
.more-info {
  height: 0px;
  display: none;
  text-align: start;
  margin-left: 20px;
  font-size: 20px;
  margin-right: 20px;
}
.show-info {
  height: fit-content;
  display: block;
}
</style>
