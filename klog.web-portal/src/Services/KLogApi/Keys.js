import KLogApiService from "../KLogApiService";

export const getApiKeys = async () => {
  return await KLogApiService.request("applications", null, "GET");
};

export const createApiKey = async (appName) => {
  return await KLogApiService.request(
    `applications?appName=${appName}`,
    null,
    "POST"
  );
};

export const deleteApiKey = async (id) => {
  return await KLogApiService.request(`applications/${id}`, null, "DELETE");
};
