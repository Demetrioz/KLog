import KLogApiService from "../KLogApiService";

export const getMostRecentLogs = async () =>
  await KLogApiService.request("logs", null, "GET");

export const getLogs = async (
  startTime,
  endTime,
  sources,
  level = null,
  searchText = null,
  searchFields = null
) => {
  let url = `logs?StartTime=${startTime}&StopTime=${endTime}&Source=${sources}`;
  if (level) url += `&LogLevel=${level}`;
  if (searchText) url += `&SearchText=${searchText}`;
  if (searchFields) url += `&SearchFields=${searchFields}`;

  return await KLogApiService.request(url, null, "GET");
};

export const getLogsByUrl = async (url) =>
  await KLogApiService.request(url, null, "GET");
