<script setup lang="ts">
import { ref, onMounted } from "vue";
import { authorized, router } from "../../main";
import { useRoute } from "vue-router";
import { AxiosResponse } from "axios";

const ids = ref<Array<string> | string>([]);
const loading = ref<boolean>(false);
const errorMessage = ref<string | null>(null);
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
  loading.value = true;
  try {
    const res = await authorized.get("shipment/generate-label", {
      params,
      responseType: "blob",
    });
    const contentType =
      res.headers["content-type"] ?? res.headers["Content-Type"];
    if (res.status < 300) {
      let date: string = new Date().toLocaleString("en-US").split(",")[0];
      let filename;
      if (contentType == "application/zip")
        filename = "Label_" + ids.value[0] + "_" + date + ".zip";
      else filename = "Label_" + ids.value[0] + "_" + date + ".pdf";
      forceDownload(res, filename);
    }
  } catch (error: any) {
    if (error.request.status == 400) {
      errorMessage.value = "Błąd pobierania etykiet";
    }
  }

  loading.value = false;
};

const redirect = () => {
  router.push("/");
};
</script>

<template>
  <div class="card">
    <div v-if="!loading" class="h-150">
      <div>
        <h2 class="pigment-green-text mt-0">Zamówienie zarejestrowane</h2>
        <div class="spacing">
          <button
            class="submit pigment-green space"
            @click="generateLabels"
            v-if="!errorMessage"
          >
            Pobierz etykietę
          </button>
          <span class="red-text" v-else>{{ errorMessage }}</span>
          <button class="submit black space" @click="redirect">Strona główna</button>
        </div>
      </div>
    </div>
    <div v-else class="h-150">
      <img src="/src/assets/loading.gif" class="loading-icon" />
    </div>
  </div>
</template>

<style scoped>
.card {
  background-color: white;
  width: 30vw;
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  border-radius: 12px;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.25);
}
button {
  width: 30% !important;
}
.h-150 {
  height: 150px;
}
.mt-0 {
  margin-top: 0;
}
.spacing {
  width: 100%;
  margin: 0 auto;
}
.space {
  margin-right: 10px;
  margin-left: 10px
}
.loading-icon {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  height: 150px;
}
</style>
