export type Shipment = {
  pickupAddress: string;
  pickupApartmentNumber: number | null;
  pickupCity: string;
  pickupPostalCode: string;
  size: number | null;
  recipientName: string;
  recipientPhoneNumber: string;
  recipientAddress: string;
  recipientApartmentNumber: number | null;
  recipientCity: string;
  recipientPostalCode: string;
  recipientEmail: string;
  additionalDetails: string | undefined;
  status: number | undefined;
};
