import KLogApiService from "./KLogApiService";

const signalR = require("@microsoft/signalr");

class SignalRService {
  static connections = {};

  static connect = (hub) => {
    let rootUrl = KLogApiService.apiUrl.replace("/api/", "");
    let endpoint = `${rootUrl}/${hub}`;

    this.connections[hub] = new signalR.HubConnectionBuilder()
      .withUrl(endpoint, { accessTokenFactory: () => KLogApiService.userToken })
      .configureLogging(signalR.LogLevel.Debug)
      .withAutomaticReconnect()
      .build();
  };

  static register = (connectionName, eventName, action) => {
    this.connections[connectionName].on(eventName, action);
  };

  static start = async (connectionName) => {
    return await this.connections[connectionName].start();
  };

  static stop = async (connectionName) => {
    await this.connections[connectionName].stop();
    delete this.connections[connectionName];
  };
}

export default SignalRService;
