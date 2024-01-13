<script setup lang="ts">
import { prefix } from "../../structures/config";
import { Courier } from "../../typings";

const props = defineProps<{
  courier: Courier;
}>();

const close_info = defineEmits(["closeModal"]);
const close = () => {
  close_info("closeModal");
};

const replaceSrc = (e: any) => {
  e.target.src = "/src/assets/account-circle.svg";
};

const formatPhoneNumber = () => {
  return (
    props.courier.phoneNumber.slice(0, 3) +
    " " +
    props.courier.phoneNumber.slice(3, 6) +
    " " +
    props.courier.phoneNumber.slice(6)
  );
};
</script>

<template>
  <div class="fog" @click.self="close">
    <div class="modal">
      <img
        :src="prefix + '/StaticFiles/' + courier.id + '.png'"
        @error="replaceSrc"
        class="profile-image"
      />
      <div class="info">
        <div>{{ props.courier.firstName }} {{ props.courier.lastName }}</div>
        <div><b>E-mail:</b> {{ props.courier.email }}</div>
        <div><b>Tel:</b> {{ formatPhoneNumber() }}</div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.modal {
  width: 40vw;
  height: fit-content;
  padding: 50px;
  z-index: 1;
  background: white;
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  margin: auto;
  color: black;
  border-radius: 12px;
  text-align: center;
  padding-top: 2%;
}

.profile-image {
  width: 30%;
  border-radius: 50%;
}

.info {
  height: 50%;
  font-size: 150%;
}
</style>
../../structures/config
