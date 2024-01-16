import { webSocketUrl } from "../../structures/config";

const getWebSocket = (message_ref: any) => {
  const webSocket = new WebSocket(webSocketUrl);
  webSocket.addEventListener("open", () => {
    webSocket.send(localStorage.getItem("token") as string);
  });

  webSocket.addEventListener("message", (event) => {
    message_ref.value = event;
  });
  console.log(webSocket);
  return webSocket;
};

export { getWebSocket };
