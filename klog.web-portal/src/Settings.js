class Settings {
  static smallWidth = 600;
  static mediumWidth = 1200;

  static getApiUrl() {
    let urlMap = {
      localhost: "https://localhost:44314/api/",
    };

    return urlMap[window.location.hostname];
  }
}

export default Settings;
