import Settings from "../Settings";

class KLogApiService {
  static apiUrl = null;
  static userToken = null;

  static Logs = require("./KLogApi/Logs");
  static Auth = require("./KLogApi/Auth");
  static Keys = require("./KLogApi/Keys");

  static async request(uri, body, method, direct = false) {
    let headers = {
      "Content-Type": "application/json",
      Authorization: `Bearer ${this.userToken}`,
    };

    let options = {
      headers: headers,
      body: method === "GET" ? null : JSON.stringify(body),
      method: method,
    };

    let url = direct ? uri : `${this.apiUrl}${uri}`;

    let response = await fetch(url, options);
    response = await response.json();

    if (response.error) throw Error(response.error);
    return response.data;
  }

  static async initialize() {
    this.apiUrl = Settings.getApiUrl();
  }
}

export default KLogApiService;
