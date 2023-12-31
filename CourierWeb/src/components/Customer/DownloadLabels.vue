<script setup lang="ts">
import { ref, onMounted } from "vue";
import { authorized, router } from "../../main";
import { useRoute } from "vue-router";
import { AxiosResponse } from "axios";

const ids = ref<Array<string> | string>([]);
const params = new URLSearchParams();
onMounted(() => {
  const items = useRoute().query.id?.valueOf();
  console.log(items);
  ids.value = Array.isArray(items) ? items : [items];
  ids.value.forEach((id) => {
    params.append("idList", id);
  });
});

const forceDownload = (response: AxiosResponse<any, any>, title: string) => {
  const url = window.URL.createObjectURL(new Blob([response.data]));
  const link = document.createElement("a");
  link.href = url;
  link.setAttribute("download", title);
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
};

const generateLabels = async () => {
  const res = await authorized.get("shipment/generate-label", {
    params,
    responseType: "blob",
  });
  if (res.status < 300) {
    let date: string = new Date().toLocaleString("en-US").split(",")[0];
    let filename = "Label_" + ids.value[0] + "_" + date + ".pdf";
    forceDownload(res, filename);
  }
};

const redirect = () => {
  router.push("/");
}
</script>

<template>
  <div class="card">
    <h2 class="pigment-green-text">Zamówienie zarejestrowane</h2>
    <button class="submit pigment-green" @click="generateLabels">
      Pobierz etykietę
    </button>
    <button class="submit" @click="redirect">Strona główna</button>
  </div>
</template>

<style>
.card {
  margin: auto;
  width: 50vw;
}
button {
  width: 30% !important;
  margin: 5% !important;
}
</style>
