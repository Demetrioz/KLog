import React, { useEffect, useState } from "react";
import { useSnackbar } from "notistack";

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

function Feed() {
  const { enqueueSnackbar } = useSnackbar();

  const [logs, setLogs] = useState([]);
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

    loadRecentLogs();
  }, [enqueueSnackbar]);

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
      if (event.target.value === "") setFilteredLogs(logs);
      else search(event.target.value);
  };

  const search = (searchString) => {
    let target = searchString.toLowerCase();
    let searchResults = [];

    logs.forEach((log) => {
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
          {logLines}
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