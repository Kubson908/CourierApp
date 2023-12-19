import { ref } from "vue";
import { Courier } from "../../typings";
import { authorized } from "../../main";

export const couriers = ref<Array<Courier>>([]);

const getCouriers = async () => {
  const res = await authorized.get("/admin/get-couriers");
  couriers.value = res.data;
};

const clearCouriers = () => {
  couriers.value = [];
};

export { getCouriers, clearCouriers };
