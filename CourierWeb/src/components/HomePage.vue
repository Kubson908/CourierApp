<script setup lang="ts">
import { onBeforeMount, ref } from "vue";
import { router, unauthorized, user } from "../main";
import { PriceList } from "../typings";

const prices = ref<PriceList | null>(null);

onBeforeMount(async () => {
  try {
    const res = await unauthorized.get("/shipment/get-price-list");
    prices.value = res.data;
  } catch (error: any) {
    if (error.status >= 400) console.log(error);
  }
});
</script>

<template>
  <div class="black-text background">
    <h1 class="pigment-green-text">O nas</h1>
    <div class="section">
      <p class="black-text justify">
        Chcesz nadać przesyłkę? Z nami Twoja paczka będzie bezpieczna. Nasz
        zespół kurierski zadba o szybką i bezpieczną dostawę. Gwarantujemy
        niezawodność i doświadczenie kurierów. Jesteśmy nowoczesną firmą, która
        idzie z duchem postępu. Wprowadzamy najnowocześniejsze rozwiązania w
        celu automatyzacji zarządzania zamówieniami. Każda przesyłka jest
        rejestrowana przez system, który ułatwia proces odbioru i dostawy, więc
        nie ma mowy o pomyłkach.
      </p>
      <img src="/kurier2.png" class="home-page-image" />
    </div>
    <div class="section">
      <button
        v-if="!user.roles.includes('Dispatcher')"
        @click="router.push('/order')"
        class="submit pigment-green main-button"
      >
        Zamów kuriera
      </button>
    </div>
    <div class="section">
      <table class="price-list">
        <thead class="table-title">
          <th colspan="5">Cennik</th>
        </thead>
        <tr>
          <th></th>
          <th>Bardzo mały</th>
          <th>Mały</th>
          <th>Średni</th>
          <th>Duży</th>
        </tr>
        <tr>
          <th>Lekka</th>
          <td>
            {{
              prices
                ? (prices.verySmallSize + prices.lightWeight)
                    .toString()
                    .substring(0, 4)
                : "8.99"
            }}
          </td>
          <td>
            {{
              prices
                ? (prices.smallSize + prices.lightWeight)
                    .toString()
                    .substring(0, 5)
                : "13.99"
            }}
          </td>
          <td>
            {{
              prices
                ? (prices.mediumSize + prices.lightWeight)
                    .toString()
                    .substring(0, 5)
                : "18.99"
            }}
          </td>
          <td>
            {{
              prices
                ? (prices.largeSize + prices.lightWeight)
                    .toString()
                    .substring(0, 5)
                : "23.99"
            }}
          </td>
        </tr>
        <tr>
          <th>Średnia</th>
          <td>
            {{
              prices
                ? (prices.verySmallSize + prices.mediumWeight)
                    .toString()
                    .substring(0, 5)
                : "11.99"
            }}
          </td>
          <td>
            {{
              prices
                ? (prices.smallSize + prices.mediumWeight)
                    .toString()
                    .substring(0, 5)
                : "16.99"
            }}
          </td>
          <td>
            {{
              prices
                ? (prices.mediumSize + prices.mediumWeight)
                    .toString()
                    .substring(0, 5)
                : "21.99"
            }}
          </td>
          <td>
            {{
              prices
                ? (prices.largeSize + prices.mediumWeight)
                    .toString()
                    .substring(0, 5)
                : "26.99"
            }}
          </td>
        </tr>
        <tr>
          <th>Ciężka</th>
          <td>
            {{
              prices
                ? (prices.verySmallSize + prices.heavyWeight)
                    .toString()
                    .substring(0, 5)
                : "15.99"
            }}
          </td>
          <td>
            {{
              prices
                ? (prices.smallSize + prices.heavyWeight)
                    .toString()
                    .substring(0, 5)
                : "20.99"
            }}
          </td>
          <td>
            {{
              prices
                ? (prices.mediumSize + prices.heavyWeight)
                    .toString()
                    .substring(0, 5)
                : "25.99"
            }}
          </td>
          <td>
            {{
              prices
                ? (prices.largeSize + prices.heavyWeight)
                    .toString()
                    .substring(0, 5)
                : "30.99"
            }}
          </td>
        </tr>
      </table>
    </div>
    <div class="section">
      <table>
        <thead class="table-title">
          <th colspan="2">Tabela rozmiarów</th>
        </thead>
        <thead>
          <th>Nazwa</th>
          <th>Wymiary</th>
        </thead>
        <tr>
          <td>Bardzo mały</td>
          <td>do 20x15x5</td>
        </tr>
        <tr>
          <td>Mały</td>
          <td>do 40x30x15</td>
        </tr>
        <tr>
          <td>Średni</td>
          <td>do 60x45x30</td>
        </tr>
        <tr>
          <td>Duży</td>
          <td>powyżej 60x45x30</td>
        </tr>
      </table>
      <table>
        <thead class="table-title">
          <th colspan="2">Tabela wag</th>
        </thead>
        <thead>
          <th>Nazwa</th>
          <th>Waga</th>
        </thead>
        <tr>
          <td>Lekka</td>
          <td>do 5 kg</td>
        </tr>
        <tr>
          <td>Średnia</td>
          <td>5 - 15 kg</td>
        </tr>
        <tr>
          <td>Ciężka</td>
          <td>powyżej 15 kg</td>
        </tr>
      </table>
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
.home-page-image {
  width: 28vh;
  height: 28vh;
  border-radius: 50%;
  background-color: #f1f1f1;
}
.section {
  display: flex;
  margin: 0 auto;
  width: 80%;
}
.justify {
  text-align: justify;
  margin-right: 20px;
  font-size: 2.1vh;
}
.main-button {
  padding: 15px 20px;
  font-size: 24px;
  color: white;
}
table {
  font-size: 24px;
  margin: 100px auto;
  width: 40%;
}
tr:nth-child(even) {
  background-color: #f2f2f2;
}
thead {
  background-color: #f2f2f2;
}
.table-title,
tr:nth-child(odd) {
  background-color: #d1d1d1;
}
.price-list {
  width: 100%;
}
.price-list td {
  width: 20%;
  padding: 20px;
  background-color: #f2f2f2;
}
.price-list th {
  background-color: #d1d1d1;
}
</style>
