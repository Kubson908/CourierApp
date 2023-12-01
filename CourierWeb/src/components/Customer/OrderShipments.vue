<script setup lang="ts">
import { ref } from "vue";
import { Shipment } from "../../typings/shipment";
import { SubmitOrder } from ".";
import { authorized, router } from "../../main";

let shipments: Array<Shipment> = new Array<Shipment>(1).fill({
  pickupAddress: "",
  pickupApartmentNumber: null,
  pickupCity: "",
  pickupPostalCode: "",
  size: null,
  weight: null,
  recipientName: "",
  recipientPhoneNumber: "",
  recipientAddress: "",
  recipientApartmentNumber: null,
  recipientCity: "",
  recipientPostalCode: "",
  recipientEmail: "",
  additionalDetails: "",
  status: undefined,
  id: undefined,
});

const activeShipment = ref<Shipment>(shipments[0]);

const pageCount = ref<number>(1);

const activePage = ref<number>(1);

const addToList: () => boolean = () => {
  if (!activeShipment.value?.size || !activeShipment.value?.weight)
    return false;
  shipments.push({
    pickupAddress:
      activeShipment.value.pickupAddress != ""
        ? activeShipment.value.pickupAddress
        : "",
    pickupApartmentNumber:
      activeShipment.value.pickupApartmentNumber != null
        ? activeShipment.value.pickupApartmentNumber
        : null,
    pickupCity:
      activeShipment.value.pickupCity != ""
        ? activeShipment.value.pickupCity
        : "",
    pickupPostalCode:
      activeShipment.value.pickupPostalCode != ""
        ? activeShipment.value.pickupPostalCode
        : "",
    size: null,
    recipientName: "",
    recipientPhoneNumber: "",
    recipientAddress: "",
    recipientApartmentNumber: null,
    recipientCity: "",
    recipientPostalCode: "",
    recipientEmail: "",
    additionalDetails: "",
    status: undefined,
    id: undefined,
    weight: null,
  });
  activeShipment.value = shipments[shipments.length - 1];
  console.log(shipments);
  return true;
};

const xs = ref<number>(0);
const s = ref<number>(0);
const m = ref<number>(0);
const l = ref<number>(0);
const submitPage = ref<boolean>(false);
const submitShipments = () => {
  if (pageCount.value > shipments.length) {
    if (!shipments[activePage.value - 1].size) return;
  }
  shipments = shipments.map((x) => {
    x.size = parseInt(x.size!.toString(), 10);
    switch (x.size) {
      case 0:
        xs.value++;
        break;
      case 1:
        s.value++;
        break;
      case 2:
        m.value++;
        break;
      case 3:
        l.value++;
        break;
    }
    return x;
  });
  console.log(xs, " ", s, " ", m, " ", l);
  submitPage.value = true;
};

const addShipment = () => {
  if (!addToList()) return;
  pageCount.value++;
  activePage.value = pageCount.value;
};

const changePage = (n: number) => {
  activePage.value = n;
  activeShipment.value = shipments[n - 1];
};

const submitOrder = async () => {
  try {
    const res = await authorized.post("/shipment/register-shipments", {
      shipments: shipments,
    });
    const shipmentsIds = res.data;
    console.log(shipmentsIds);
    if (res.status < 300) {
      submitPage.value = false;
      router.push({ path: "/order-registered", query: { id: shipmentsIds } });
    }
  } catch (error: any) {
    console.log(error);
  }
};
</script>

<template>
  <div class="card" v-if="!submitPage">
    <div v-if="pageCount > 1">
      <label
        class="page-label"
        v-for="i in pageCount"
        @click="changePage(i)"
        :class="i == activePage ? 'active-page' : 'page-label'"
        >{{ i }}</label
      >
    </div>
    <div>
      <form class="flex-col">
        <h2 class="pigment-green-text">Dane nadawcy</h2>
        <table>
          <tr>
            <td>
              <input
                type="text"
                v-model="activeShipment!.pickupAddress"
                placeholder="Adres odbioru"
                class="rounded-input"
              />
            </td>
            <td>
              <input
                type="number"
                v-model="activeShipment!.pickupApartmentNumber"
                placeholder="Numer mieszkania"
                class="rounded-input"
              />
            </td>
          </tr>
          <tr>
            <td>
              <input
                type="text"
                v-model="activeShipment!.pickupCity"
                placeholder="Miejscowość"
                class="rounded-input"
              />
            </td>
            <td>
              <input
                type="text"
                v-model="activeShipment!.pickupPostalCode"
                placeholder="Kod pocztowy"
                class="rounded-input"
              />
            </td>
          </tr>
        </table>
        <h2 class="pigment-green-text">Dane odbiorcy</h2>
        <table>
          <tr>
            <td>
              <input
                type="text"
                v-model="activeShipment!.recipientName"
                placeholder="Odbiorca"
                class="rounded-input"
              />
            </td>
            <td>
              <input
                type="text"
                v-model="activeShipment!.recipientPhoneNumber"
                placeholder="Telefon"
                class="rounded-input"
              />
            </td>
          </tr>
          <tr>
            <td>
              <input
                type="text"
                v-model="activeShipment!.recipientAddress"
                placeholder="Adres"
                class="rounded-input"
              />
            </td>
            <td>
              <input
                type="number"
                v-model="activeShipment!.recipientApartmentNumber"
                placeholder="Numer mieszkania"
                class="rounded-input"
              />
            </td>
          </tr>
          <tr>
            <td>
              <input
                type="text"
                v-model="activeShipment!.recipientCity"
                placeholder="Miejscowość"
                class="rounded-input"
              />
            </td>
            <td>
              <input
                type="text"
                v-model="activeShipment!.recipientPostalCode"
                placeholder="Kod pocztowy"
                class="rounded-input"
              />
            </td>
          </tr>
          <tr>
            <td colspan="2">
              <input
                type="text"
                v-model="activeShipment!.recipientEmail"
                placeholder="Email"
                class="rounded-input"
              />
            </td>
          </tr>
          <tr></tr>
          <tr>
            <td>
              <select
                v-model="activeShipment!.size"
                required
                :class="activeShipment!.size == null ? 'gray' : 'black'"
              >
                <option value="null" selected hidden>Rozmiar przesyłki</option>
                <option value="0" class="option">Bardzo mały</option>
                <option value="1" class="option">Mały</option>
                <option value="2" class="option">Średni</option>
                <option value="3" class="option">Duży</option>
              </select>
            </td>
            <td>
              <input
                type="number"
                v-model="activeShipment!.weight"
                placeholder="Waga przesyłki"
                class="rounded-input"
              />
            </td>
          </tr>
        </table>

        <textarea
          type="text"
          v-model="activeShipment!.additionalDetails"
          placeholder="Dodatkowe informacje"
          class="textarea"
        ></textarea>
      </form>
      <button @click="addShipment">+</button>
      <button class="submit pigment-green" @click="submitShipments">
        Zatwierdź
      </button>
    </div>
  </div>
  <SubmitOrder
    v-else
    :xs="xs"
    :s="s"
    :m="m"
    :l="l"
    @cancel="submitPage = false"
    @submit="submitOrder"
  />
</template>

<style scoped>
.gray {
  color: gray;
}
.card {
  background-color: white;
  width: 30vw;
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  border-radius: 12px;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.25);
  flex-direction: column;
  display: flex;
}

.textarea {
  resize: none;
  border-radius: 10px;
  background-color: #f6f6f6;
  border: solid 2px #e8e8e8;
  height: 8vh;
  margin-top: 3vh;
  font-family: Arial, Helvetica, sans-serif;
  color: black;
}
.textarea::placeholder {
  font-family: Arial, Helvetica, sans-serif;
}
.page-label {
  background-color: red;
  float: left;
  margin: 0 5px;
  cursor: pointer;
}
.active-page {
  background-color: green;
}

.rounded-input {
  border-radius: 10px;
  height: 30px;
  width: 90%;
  margin-top: 0.5vh;
  background-color: #f6f6f6;
  border: solid 2px #e8e8e8;
  color: black;
}

.option {
  color: black;
  border-radius: 5px;
  background-color: #e8e8e8;
}

select {
  border-radius: 10px;
  height: 35px;
  width: 90%;
  margin-top: 0.5vh;
  background-color: #f6f6f6;
  border: solid 2px #e8e8e8;
  color: black;
}
</style>
