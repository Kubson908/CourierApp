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

// function clearActiveShipment() {
//   var s: Shipment = {
//     pickupAddress: "",
//     pickupApartmentNumber: null,
//     pickupCity: "",
//     pickupPostalCode: "",
//     size: null,
//     recipientName: "",
//     recipientPhoneNumber: "",
//     recipientAddress: "",
//     recipientApartmentNumber: null,
//     recipientCity: "",
//     recipientPostalCode: "",
//     recipientEmail: "",
//     additionalDetails: "",
//   };
//   activeShipment.value = s;
// }

// function clearRecipientData() {
//   activeShipment.value!.size = null;
//   activeShipment.value!.recipientName = "";
//   activeShipment.value!.recipientPhoneNumber = "";
//   activeShipment.value!.recipientAddress = "";
//   activeShipment.value!.recipientApartmentNumber = null;
//   activeShipment.value!.recipientCity = "";
//   activeShipment.value!.recipientPostalCode = "";
//   activeShipment.value!.recipientEmail = "";
//   activeShipment.value!.additionalDetails = "";
// }

const pageCount = ref<number>(1);

const activePage = ref<number>(1);

// const pickupAddress = ref<string>("");
// const pickupApartmentNumber = ref<number | null>(null);
// const pickupCity = ref<string>("");
// const pickupPostalCode = ref<string>("");
// const size = ref<number | null>(null);
// const recipientName = ref<string>("");
// const recipientPhoneNumber = ref<string>("");
// const recipientAddress = ref<string>("");
// const recipientApartmentNumber = ref<number | null>(null);
// const recipientCity = ref<string>("");
// const recipientPostalCode = ref<string>("");
// const recipientEmail = ref<string>("");
// const additionalDetails = ref<string | undefined>(undefined);

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
    return x;
  });
  xs.value = shipments.filter((shipment) => {
    return shipment.size == 0;
  }).length;
  s.value = shipments.filter((shipment) => {
    return shipment.size == 1;
  }).length;
  m.value = shipments.filter((shipment) => {
    return shipment.size == 2;
  }).length;
  l.value = shipments.filter((shipment) => {
    return shipment.size == 3;
  }).length;
  submitPage.value = true;
  console.log(shipments); // console log do wyjebania
};

const addShipment = () => {
  if (!addToList()) return;
  pageCount.value++;
  activePage.value = pageCount.value;
  // clearRecipientData();
};

const changePage = (n: number) => {
  activePage.value = n;
  activeShipment.value = shipments[n - 1];
  // if (shipments[n - 1]) {
  //   activeShipment.value = { ...shipments[n - 1] };
  //   console.log(shipments[n - 1]);
  // } else if (activeShipment.value?.pickupAddress != "") {
  //   shipments[n - 1] = { ...activeShipment.value };
  //   clearRecipientData();
  //   console.log("ssdfsddsfdsfgdgfdgfdhfgf");
  // } else clearActiveShipment();
  // console.log(activeShipment.value);
  // console.log(shipments[n - 1]);
};

const submitOrder = async () => {
  try {
    const res = await authorized.post("/shipment/register-shipments", {
      shipments: shipments,
    });
    if (res.status < 300) router.push("/");
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
        <input
          type="text"
          v-model="activeShipment!.pickupAddress"
          placeholder="Adres odbioru"
        />
        <input
          type="number"
          v-model="activeShipment!.pickupApartmentNumber"
          placeholder="Numer mieszkania"
        />
        <input
          type="text"
          v-model="activeShipment!.pickupCity"
          placeholder="Miejscowość"
        />
        <input
          type="text"
          v-model="activeShipment!.pickupPostalCode"
          placeholder="Kod pocztowy"
        />
        <h2 class="pigment-green-text">Dane odbiorcy</h2>
        <input
          class="test"
          type="text"
          v-model="activeShipment!.recipientName"
          placeholder="Odbiorca"
        />
        <input
          type="text"
          v-model="activeShipment!.recipientPhoneNumber"
          placeholder="Telefon"
        />
        <input
          type="text"
          v-model="activeShipment!.recipientAddress"
          placeholder="Adres"
        />
        <input
          type="number"
          v-model="activeShipment!.recipientApartmentNumber"
          placeholder="Numer mieszkania"
        />
        <input
          type="text"
          v-model="activeShipment!.recipientCity"
          placeholder="Miejscowość"
        />
        <input
          type="text"
          v-model="activeShipment!.recipientPostalCode"
          placeholder="Kod pocztowy"
        />
        <input
          type="text"
          v-model="activeShipment!.recipientEmail"
          placeholder="Email"
        />
        <!-- <input
          type="number"
          v-model="activeShipment!.size"
          placeholder="Rozmiar"
        /> -->
        <select
          v-model="activeShipment!.size"
          required
          :class="activeShipment!.size == null ? 'gray' : 'white'"
        >
          <option value="null" selected hidden>Rozmiar</option>  
          <option value="0">Bardzo mały</option>
          <option value="1">Mały</option>
          <option value="2">Średni</option>
          <option value="3">Duży</option>
        </select>
        <input
          type="number"
          v-model="activeShipment!.weight"
          placeholder="Waga"
        />
        <textarea
          type="text"
          v-model="activeShipment!.additionalDetails"
          placeholder="Dodatkowe informacje"
          class="textarea"
        ></textarea>
      </form>
      <!-- <div class="flex-col" v-else>
        <h2 class="pigment-green-text">Dane nadawcy</h2>
        <input
          type="text"
          v-model="activeShipment!.pickupAddress"
          placeholder="Adres odbioru"
        />
        <input
          type="text"
          v-model="activeShipment!.pickupApartmentNumber"
          placeholder="Numer mieszkania"
        />
        <input
          type="text"
          v-model="activeShipment!.pickupCity"
          placeholder="Miejscowość"
        />
        <input
          type="text"
          v-model="activeShipment!.pickupPostalCode"
          placeholder="Kod pocztowy"
        />
        <h2 class="pigment-green-text">Dane odbiorcy</h2>
        <input
          type="text"
          v-model="activeShipment!.recipientName"
          placeholder="Odbiorca"
        />
        <input
          type="text"
          v-model="activeShipment!.recipientPhoneNumber"
          placeholder="Telefon"
        />
        <input
          type="text"
          v-model="activeShipment!.recipientAddress"
          placeholder="Adres"
        />
        <input
          type="number"
          v-model="activeShipment!.recipientApartmentNumber"
          placeholder="Numer mieszkania"
        />
        <input
          type="text"
          v-model="activeShipment!.recipientCity"
          placeholder="Miejscowość"
        />
        <input
          type="text"
          v-model="activeShipment!.recipientPostalCode"
          placeholder="Kod pocztowy"
        />
        <input
          type="text"
          v-model="activeShipment!.recipientEmail"
          placeholder="Email"
        />
        <input
          type="number"
          v-model="activeShipment!.size"
          placeholder="Rozmiar"
        />
        <textarea
          type="text"
          v-model="activeShipment!.additionalDetails"
          placeholder="Dodatkowe informacje"
          class="textarea"
        ></textarea>
      </div> -->
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
option {
  color: white;
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
  height: 8vh;
  margin-top: 3vh;
  font-family: Arial, Helvetica, sans-serif;
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
</style>
