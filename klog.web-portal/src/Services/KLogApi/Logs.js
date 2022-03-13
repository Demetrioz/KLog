import KLogApiService from "../KLogApiService";

export const getMostRecentLogs = async () => {
  return await KLogApiService.request("logs", null, "GET");
};
