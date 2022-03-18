import React, { useEffect, useState } from "react";
import { useSnackbar } from "notistack";
import { clone } from "lodash";

import Checkbox from "@mui/material/Checkbox";
import IconButton from "@mui/material/IconButton";
import InputAdornment from "@mui/material/InputAdornment";
import Menu from "@mui/material/Menu";
import MenuItem from "@mui/material/MenuItem";
import TextField from "@mui/material/TextField";

import FilterIcon from "@mui/icons-material/FilterList";
import SearchIcon from "@mui/icons-material/SearchRounded";
import VisibilityIcon from "@mui/icons-material/Visibility";

import BasePage from "../BasePage/BasePage";

import KLogApiService from "../../Services/KLogApiService";
import SignalRService from "../../Services/SignalRService";
import { useAsyncRef } from "../../Utilities/Hooks";

function Feed() {
  const { enqueueSnackbar } = useSnackbar();

  const [logs, setLogs] = useAsyncRef([]);
  const [searchString, setSearchString] = useAsyncRef("");
  const [filteredLogs, setFilteredLogs] = useState([]);

  const [sourceFilter, setSourceFilter] = useState(true);
  const [subjectFilter, setSubjectFilter] = useState(true);
  const [levelFilter, setLevelFilter] = useState(true);
  const [messageFilter, setMessageFilter] = useState(true);
  const [filterAnchor, setFilterAnchor] = useState(null);

  const [source, setSource] = useState(true);
  const [subject, setSubject] = useState(true);
  const [level, setLevel] = useState(true);
  const [message, setMessage] = useState(true);
  const [visibilityAnchor, setVisibilityAnchor] = useState(null);

  const filterOpen = Boolean(filterAnchor);
  const visibilityOpen = Boolean(visibilityAnchor);

  // Put the most recent logs on top. The div's direction (column-reverse)
  // will then put the newest on the bottom and always scroll, as intended
  const logLines = filteredLogs
    .sort((a, b) => a.timestamp < b.timestamp)
    .map((log) => (
      <div key={log.logId}>
        {log.timestamp} || {level && log.level + " || "}
        {source && log.source + " || "}
        {subject && log.subject + " || "} {message && log.message}
      </div>
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
  }, [enqueueSnackbar, handleSearch, logs, searchString, setLogs]);

  const handleFilterClick = (event) => {
    setFilterAnchor(event.currentTarget);
  };

  const handleVisibilityClick = (event) => {
    setVisibilityAnchor(event.currentTarget);
  };

  const handleClose = () => {
    setFilterAnchor(null);
    setVisibilityAnchor(null);
  };

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
          <TextField
            id="search"
            variant="outlined"
            margin="normal"
            onKeyDown={handleSearch}
            InputProps={{
              startAdornment: (
                <InputAdornment position="start">
                  <SearchIcon />
                </InputAdornment>
              ),
              endAdornment: (
                <InputAdornment position="end">
                  <IconButton aria-label="filter" onClick={handleFilterClick}>
                    <FilterIcon />
                  </IconButton>
                  <IconButton
                    aria-label="display"
                    onClick={handleVisibilityClick}
                  >
                    <VisibilityIcon />
                  </IconButton>
                  <Menu
                    anchorEl={filterAnchor}
                    open={filterOpen}
                    onClose={handleClose}
                  >
                    <MenuItem onClick={() => setSourceFilter(!sourceFilter)}>
                      <Checkbox checked={sourceFilter} />
                      Source
                    </MenuItem>
                    <MenuItem onClick={() => setSubjectFilter(!subjectFilter)}>
                      <Checkbox checked={subjectFilter} />
                      Subject
                    </MenuItem>
                    <MenuItem onClick={() => setLevelFilter(!levelFilter)}>
                      <Checkbox checked={levelFilter} />
                      Level
                    </MenuItem>
                    <MenuItem onClick={() => setMessageFilter(!messageFilter)}>
                      <Checkbox checked={messageFilter} />
                      Message
                    </MenuItem>
                  </Menu>
                  <Menu
                    anchorEl={visibilityAnchor}
                    open={visibilityOpen}
                    onClose={handleClose}
                  >
                    <MenuItem onClick={() => setSource(!source)}>
                      <Checkbox checked={source} />
                      Source
                    </MenuItem>
                    <MenuItem onClick={() => setSubject(!subject)}>
                      <Checkbox checked={subject} />
                      Subject
                    </MenuItem>
                    <MenuItem onClick={() => setLevel(!level)}>
                      <Checkbox checked={level} />
                      Level
                    </MenuItem>
                    <MenuItem onClick={() => setMessage(!message)}>
                      <Checkbox checked={message} />
                      Message
                    </MenuItem>
                  </Menu>
                </InputAdornment>
              ),
            }}
          />
        </div>
      }
    />
  );
}

export default Feed;
