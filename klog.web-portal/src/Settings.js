class Settings {
  static version = "0.1.0";

  static smallWidth = 600;
  static mediumWidth = 1200;

  static getApiUrl() {
    return window.location.port === 3000
      ? "https://localhost:44314/api/"
      : `https://${window.location.hostname}:${window.location.port}/api/`;
  }
}

export default Settings;
