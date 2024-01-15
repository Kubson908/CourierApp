<script setup lang="ts">
import { onBeforeMount, ref } from "vue";
import { Shipment } from "../../typings/shipment";
import { SubmitOrder } from ".";
import { authorized, priceList, router } from "../../main";

const sizePrices = ref<Array<number>>();
const weightPrices = ref<Array<number>>();

onBeforeMount(async () => {
  const res = await authorized.get("/shipment/get-price-list");
  if (res.status == 200) priceList.value = res.data;
  sizePrices.value = [
    priceList.value!.verySmallSize,
    priceList.value!.smallSize,
    priceList.value!.mediumSize,
    priceList.value!.largeSize,
  ];
  weightPrices.value = [
    priceList.value!.lightWeight,
    priceList.value!.mediumWeight,
    priceList.value!.heavyWeight,
  ];
});

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
  price: undefined,
  deliveryAttempts: undefined,
});

const activeShipment = ref<Shipment>(shipments[0]);

const pageCount = ref<number>(1);

const activePage = ref<number>(1);

const verifyShipment: () => boolean = () => {
  const excludedFields: Array<string> = [
    "id",
    "pickupApartmentNumber",
    "recipientApartmentNumber",
    "status",
    "price",
    "additionalDetails",
    "deliveryAttempts",
  ];
  let shipment = shipments[shipments.length - 1];
  let status: boolean = true;
  Object.keys(shipment)
    .filter((property) => !excludedFields.includes(property))
    .forEach((property) => {
      const value = shipment[property as keyof Shipment];
      if (value == null || value == "") {
        status = false;
        return;
      }
    });
  if (!status) return false;
  return true;
};

const addToList: () => boolean = () => {
  if (!verifyShipment()) return false;
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
    price: undefined,
    deliveryAttempts: undefined,
  });
  activeShipment.value = shipments[shipments.length - 1];
  return true;
};

const xs = ref<number>(0);
const s = ref<number>(0);
const m = ref<number>(0);
const l = ref<number>(0);
const submitPage = ref<boolean>(false);
const submitShipments = () => {
  if (!verifyShipment()) return;
  shipments = shipments.map((x) => {
    x.size = parseInt(x.size!.toString(), 10);
    x.weight = parseInt(x.weight!.toString(), 10);
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
    if (res.status < 300) {
      submitPage.value = false;
      router.push({ path: "/order-registered", query: { id: shipmentsIds } });
    }
  } catch (error: any) {
    console.log(error);
  }
};

const managePrice = () => {
  if (!activeShipment.value.weight || !activeShipment.value.size) return;
  activeShipment.value.price = parseFloat(
    (
      sizePrices.value![activeShipment.value.size] +
      weightPrices.value![activeShipment.value.weight]
    ).toFixed(2)
  );
};

const removeShipment = () => {
  let previous: number = activePage.value - 1;
  if (activePage.value > 1) activePage.value--;
  else activePage.value++;
  activeShipment.value = shipments[activePage.value - 1];
  shipments.splice(previous, 1);
  pageCount.value--;
}
</script>

<template>
  <div class="card" v-if="!submitPage">
    <div v-if="pageCount > 1" class="page-numbers">
      <label
        class="page-label pigment-green-text"
        v-for="i in pageCount"
        @click="changePage(i)"
        :class="i == activePage ? 'active-page' : 'page-label'"
        >{{ i }}</label
      >
      <button class="right-corner submit" @click="removeShipment">
        <img src="/src/assets/delete.svg" :width="20" />
      </button>
    </div>
    <div>
      <form class="flex-col">
        <h2 class="pigment-green-text">Dane nadawcy</h2>
        <table>
          <tr>
            <td>
              <input
                name="pickupAddress"
                type="text"
                v-model="activeShipment!.pickupAddress"
                placeholder="Adres odbioru"
                class="rounded-input"
              />
            </td>
            <td>
              <input
                name=" pickupAppartmentNumber"
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
                name="pickupCity"
                type="text"
                v-model="activeShipment!.pickupCity"
                placeholder="Miejscowość"
                class="rounded-input"
              />
            </td>
            <td>
              <input
                name="pickupPostalCode"
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
                name="recipientName"
                type="text"
                v-model="activeShipment!.recipientName"
                placeholder="Odbiorca"
                class="rounded-input"
              />
            </td>
            <td>
              <input
                name="recipientPhoneNumber"
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
                name="recipientAddress"
                type="text"
                v-model="activeShipment!.recipientAddress"
                placeholder="Adres"
                class="rounded-input"
              />
            </td>
            <td>
              <input
                name="recipientApartmentNumber"
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
                name="recipientCity"
                type="text"
                v-model="activeShipment!.recipientCity"
                placeholder="Miejscowość"
                class="rounded-input"
              />
            </td>
            <td>
              <input
                name="recipientPostalCode"
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
                name="recipientEmail"
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
                name="size"
                v-model="activeShipment!.size"
                required
                @change="managePrice"
                :class="activeShipment!.size == null ? 'gray' : 'black'"
              >
                <option value="null" selected hidden>Rozmiar przesyłki</option>
                <option value="0" class="option">Bardzo mały (do 20x15x5)</option>
                <option value="1" class="option">Mały (do 40x30x15)</option>
                <option value="2" class="option">Średni (do 60x45x30)</option>
                <option value="3" class="option">Duży (powyżej 60x45x30)</option>
              </select>
            </td>
            <td>
              <select
                name="weight"
                v-model="activeShipment!.weight"
                required
                @change="managePrice"
                :class="activeShipment!.weight == null ? 'gray' : 'black'"
              >
                <option value="null" selected hidden>Waga</option>
                <option value="0" class="option">Lekka (do 5 kg)</option>
                <option value="1" class="option">Średnia (5 - 15 kg)</option>
                <option value="2" class="option">Ciężka (powyżej 15 kg)</option>
              </select>
            </td>
          </tr>
        </table>

        <textarea
          name="additionalDetails"
          type="text"
          v-model="activeShipment!.additionalDetails"
          placeholder="Dodatkowe informacje"
          class="textarea"
        ></textarea>
      </form>
      <div>
        <span v-if="activeShipment!.price" class="black-text left">
          Cena przesyłki: {{ activeShipment!.price }} zł
        </span>
        <span class="black-text right" v-if="shipments[0].price"> Suma: {{
          (shipments.map((s) => s.price).filter((p) => p != undefined) as Array<number>).reduce((a, b) => a + b, 0).toString().substring(0, 6)
        }} zł</span>
      </div>
      <br />
      <button @click="addShipment">Dodaj</button>
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
  max-height: 75vh;
  overflow-y: auto;
}
.card::-webkit-scrollbar {
  width: 10px;
}
.card::-webkit-scrollbar-track {
  border-radius: 10px;
  background: #BDBDBD;
}
.card::-webkit-scrollbar-thumb {
  background: #15AB54;
  border-radius: 10px;
}
.card::-webkit-scrollbar-thumb:hover {
  background: #129448;
}

.textarea {
  resize: none;
  border-radius: 10px;
  background-color: #f6f6f6;
  border: solid 2px #e8e8e8;
  height: 8vh;
  margin-top: 1vh;
  font-family: Arial, Helvetica, sans-serif;
  color: black;
}
.textarea::placeholder {
  font-family: Arial, Helvetica, sans-serif;
}

.page-numbers {
  width: 90%;
  margin: auto;
}

.right-corner {
  position: fixed;
  top: 0;
  right: 0;
  height: 40px;
  width: 40px !important;
  margin: 10px !important;
  padding: 7px !important;
}

.page-label {
  font-weight:bold;
  padding: 3px 10px;
  border-radius: 10%;
  background-color: #e9e9e9;
  float: left;
  margin: 5px 5px;
  cursor: pointer;
}
.active-page {
  color: white;
  padding: 3px 10px;
  background-color: #848C8E;
  box-shadow: 0px 0px 10px rgba(0, 0, 0, 1);
}
.page-label:hover {
  scale: 1.1;
}
.page-label:active {
  scale: 1;
}
.active-page:hover {
  scale: 1;
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

.left {
  margin-left: 15px;
  float: left;
}

.right {
  margin-right: 15px;
  float: right;
}
</style>
