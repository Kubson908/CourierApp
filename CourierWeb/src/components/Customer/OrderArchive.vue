<script setup lang="ts">
import { onBeforeMount, ref } from "vue";
import { Order } from "../../typings";
import { authorized } from "../../main";
import { format } from "date-fns";
import { pl } from "date-fns/locale";

const loading = ref<boolean>(false);

const orders = ref<Array<Order>>([]);

onBeforeMount(async () => {
  loading.value = true;
  try {
    const res = await authorized.get("/shipment/get-orders");
    orders.value = res.data;
  } catch {}
  loading.value = false;
});

const formatDate = (date: string) => {
  return format(new Date(date), "eeee, dd.MM.yyyy, HH:mm", { locale: pl });
};
</script>

<template>
  <div class="black-text background">
    <div v-if="!loading">
      <h1 class="pigment-green-text">Historia zamówień</h1>
      <div v-for="order in orders" v-if="orders.length > 0">
        <div class="list-element">
          <span class="col">Ilość przesyłek: {{ order.shipmentCount }}</span>
          <span class="col">{{ formatDate(order.orderDate) }}</span>
          <span class="col">
            <a :href="'/history/details?id=' + order.id" class="pointer"
              >Szczegóły</a
            >
          </span>
        </div>
        <hr v-if="orders[orders.length - 1] != order" class="divider" />
      </div>
      <h2 v-else class="black-text">Brak zamówień w historii</h2>
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
.list-element {
  margin-left: 20px;
  margin-right: 20px;
  display: flex;
  height: 50px;
  font-size: 20px;
  background-color: #f1f1f1;
}
.divider {
  margin-left: 20px;
  margin-right: 20px;
}
.col {
  width: 33%;
  align-self: center;
}
.pointer {
  cursor: pointer;
  -webkit-user-select: none; /* Safari */
  -ms-user-select: none; /* IE 10 and IE 11 */
  user-select: none; /* Standard syntax */
}
.pointer:hover {
  text-decoration: underline;
}
.pointer:active {
  color: purple;
}
</style>
