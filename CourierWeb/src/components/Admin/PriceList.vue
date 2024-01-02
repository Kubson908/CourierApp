<script setup lang="ts">
import { onBeforeMount, ref } from "vue";
import { authorized, priceList } from "../../main";

const fetchData = async () => {
  const res = await authorized.get("/shipment/get-price-list");
  if (res.status == 200) priceList.value = res.data;
};

onBeforeMount(async () => {
  loading.value = true;
  await fetchData();
  loading.value = false;
});

const loading = ref<boolean>(false);

const submit = async () => {
  loading.value = true;
  try {
    const res = await authorized.patch("/admin/update-price-list", {
      verySmallSize: priceList.value!.verySmallSize,
      smallSize: priceList.value!.smallSize,
      mediumSize: priceList.value!.mediumSize,
      largeSize: priceList.value!.largeSize,
      lightWeight: priceList.value!.lightWeight,
      mediumWeight: priceList.value!.mediumWeight,
      heavyWeight: priceList.value!.heavyWeight,
    });
    if (res.status == 200) {
      loading.value = false;
      await fetchData();
    }
  } catch {
    loading.value = false;
  }
};
</script>

<template>
  <div class="card">
    <div v-if="!loading">
      <h2 class="center pigment-green-text">Cennik</h2>
      <p class="black-text large-font">
        Cena przesyłki jest sumą ceny za rozmiar i wagę.
      </p>
      <h2 class="center pigment-green-text">Rozmiar</h2>
      <div v-if="priceList">
        <table>
          <tr>
            <td class="black-text title">XS</td>
            <td class="value">
              <input
                v-model="priceList.verySmallSize"
                class="rounded-input center"
              />
            </td>
            <td class="black-text currency">zł</td>
          </tr>
          <tr>
            <td class="black-text title">S</td>
            <td class="value">
              <input
                v-model="priceList.smallSize"
                class="rounded-input center"
              />
            </td>
            <td class="black-text currency">zł</td>
          </tr>
          <tr>
            <td class="black-text title">M</td>
            <td class="value">
              <input
                v-model="priceList.mediumSize"
                class="rounded-input center"
              />
            </td>
            <td class="black-text currency">zł</td>
          </tr>
          <tr>
            <td class="black-text title">L</td>
            <td class="value">
              <input
                v-model="priceList.largeSize"
                class="rounded-input center"
              />
            </td>
            <td class="black-text currency">zł</td>
          </tr>
        </table>
        <h2 class="center pigment-green-text">Waga</h2>
        <table>
          <tr>
            <td class="black-text title">Lekka</td>
            <td class="value">
              <input
                v-model="priceList.lightWeight"
                class="rounded-input center"
              />
            </td>
            <td class="black-text currency">zł</td>
          </tr>
          <tr>
            <td class="black-text title">Średnia</td>
            <td class="value">
              <input
                v-model="priceList.mediumWeight"
                class="rounded-input center"
              />
            </td>
            <td class="black-text currency">zł</td>
          </tr>
          <tr>
            <td class="black-text title">Ciężka</td>
            <td class="value">
              <input
                v-model="priceList.heavyWeight"
                class="rounded-input center"
              />
            </td>
            <td class="black-text currency">zł</td>
          </tr>
        </table>
      </div>
      <button class="submit pigment-green center mt-10" @click="submit">
        Zapisz
      </button>
    </div>
    <div v-else>
      <img src="/src/assets/loading.gif" class="loading-icon" />
    </div>
  </div>
</template>

<style scoped>
.card {
  background-color: white;
  width: 20vw;
  min-height: 520px !important;
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  border-radius: 12px;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.25);
  flex-direction: column;
  display: flex;
  max-height: 75vh;
  overflow-y: auto;
}
.rounded-input {
  border-radius: 10px;
  height: 30px;
  margin: 0;
  background-color: #f6f6f6;
  border: solid 2px #e8e8e8;
  color: black;
  font-size: 24px;
  width: 90%;
  text-align: right;
}
.label {
  margin-bottom: 0;
  font-size: small;
  color: rgb(85, 85, 85);
  margin-left: 15%;
}
.title {
  text-align: right;
  width: 20%;
  font-size: 20px;
}
.value {
  width: 30%;
}
.currency {
  width: 20%;
  text-align: left;
  font-size: 20px;
}
.large-font {
  font-size: large;
}
.loading-icon {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
}
</style>
