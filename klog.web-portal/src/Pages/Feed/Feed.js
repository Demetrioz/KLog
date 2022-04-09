import React, { useEffect, useState } from "react";
import { useSnackbar } from "notistack";
import { clone, orderBy } from "lodash";

import LogSearchBar from "../../Components/LogSearchBar/LogSearchBar";

import BasePage from "../BasePage/BasePage";
import LogLine from "../../Components/LogLine/LogLine";

import KLogApiService from "../../Services/KLogApiService";
import SignalRService from "../../Services/SignalRService";
import { useAsyncRef } from "../../Utilities/Hooks";

function Feed() {
  const { enqueueSnackbar } = useSnackbar();

  const [logs, setLogs] = useAsyncRef([]);
  const [searchString, setSearchString] = useAsyncRef("");
  const [filteredLogs, setFilteredLogs] = useState([]);

  // Search / filter options
  const [sourceFilter, setSourceFilter] = useState(true);
  const [subjectFilter, setSubjectFilter] = useState(true);
  const [levelFilter, setLevelFilter] = useState(true);
  const [messageFilter, setMessageFilter] = useState(true);

  // Visibility options
  const [source, setSource] = useState(true);
  const [subject, setSubject] = useState(true);
  const [level, setLevel] = useState(true);
  const [message, setMessage] = useState(true);

  // Put the most recent logs on top. The div's direction (column-reverse)
  // will then put the newest on the bottom and always scroll, as intended
  const logLines = filteredLogs
    .sort((a, b) => a.timestamp < b.timestamp)
    .map((log) => (
      <LogLine
        key={log.logId}
        log={log}
        showLevel={level}
        showSource={source}
        showSubject={subject}
        showMessage={message}
      />
    ));

  useEffect(() => {
    const connectToHub = async () => {
      try {
        SignalRService.connect("logHub");
        SignalRService.register("logHub", "PublishLog", handleNewLog);
        await SignalRService.start("logHub");
      } catch (e) {
        enqueueSnackbar(e.message, { variant: "error" });
      }
    };

    const disconnectFromHub = async () => {
      try {
        await SignalRService.stop("logHub");
      } catch (e) {
        enqueueSnackbar(e.message, { variant: "error" });
      }
    };

    const handleNewLog = (log) => {
      let updatedLogs = clone(logs.current);
      updatedLogs.push(log);
      updatedLogs = orderBy(updatedLogs, ["timestamp"], ["desc"]);
      setLogs(updatedLogs);
      handleSearch({ key: "Enter", target: { value: searchString.current } });
    };

    const loadRecentLogs = async () => {
      try {
        let logs = await KLogApiService.Logs.getMostRecentLogs();

        if (!Array.isArray(logs.items))
          throw new Error("Received invalid logs!");

        setLogs(logs.items);
        setFilteredLogs(logs.items);
      } catch (e) {
        enqueueSnackbar(e.message, { variant: "error" });
      }
    };

    connectToHub();
    loadRecentLogs();

    return disconnectFromHub;
  }, []);

  const handleSearch = (event) => {
    if (event.key === "Enter")
      if (event.target.value === "") {
        setFilteredLogs(logs.current);
        setSearchString(event.target.value);
      } else search(event.target.value);
  };

  const search = (searchString) => {
    setSearchString(searchString);
    let target = searchString.toLowerCase();
    let searchResults = [];

    logs.current.forEach((log) => {
      let logSource = log.source?.toLowerCase() ?? "";
      let logSubject = log.subject?.toLowerCase() ?? "";
      let logLevel = log.level?.toLowerCase() ?? "";
      let logMessage = log.message?.toLowerCase() ?? "";

      if (
        (sourceFilter && logSource.includes(target)) ||
        (subjectFilter && logSubject.includes(target)) ||
        (levelFilter && logLevel.includes(target)) ||
        (messageFilter && logMessage.includes(target))
      )
        searchResults.push(log);
    });

    setFilteredLogs(searchResults);
  };

  return (
    <BasePage
      title="Log Feed"
      content={
        <div
          id="feed_container"
          style={{ height: "100%", display: "flex", flexDirection: "column" }}
        >
          <div id="spacer" style={{ flexGrow: 1 }}></div>
          <div
            id="logs"
            style={{
              overflowY: "auto",
              display: "flex",
              flexDirection: "column-reverse",
            }}
          >
            {logLines}
          </div>
          <LogSearchBar
            onTextKeyDown={handleSearch}
            searchOptions={[
              {
                id: "source",
                value: sourceFilter,
                text: "Source",
                handler: setSourceFilter,
              },
              {
                id: "subject",
                value: subjectFilter,
                text: "Subject",
                handler: setSubjectFilter,
              },
              {
                id: "level",
                value: levelFilter,
                text: "Level",
                handler: setLevelFilter,
              },
              {
                id: "message",
                value: messageFilter,
                text: "Message",
                handler: setMessageFilter,
              },
            ]}
            visibilityOptions={[
              {
                id: "source",
                value: source,
                text: "Source",
                handler: setSource,
              },
              {
                id: "subject",
                value: subject,
                text: "Subject",
                handler: setSubject,
              },
              {
                id: "level",
                value: level,
                text: "Level",
                handler: setLevel,
              },
              {
                id: "message",
                value: message,
                text: "Message",
                handler: setMessage,
              },
            ]}
          />
        </div>
      }
    />
  );
}

export default Feed;
