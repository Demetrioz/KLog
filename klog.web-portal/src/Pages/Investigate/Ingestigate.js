import React, { useEffect, useState } from "react";
import { useSnackbar } from "notistack";
import { DataGrid } from "@mui/x-data-grid";
import { useTheme } from "@mui/material";
import { find } from "lodash";
import moment from "moment";

import LogSearchBar from "../../Components/LogSearchBar/LogSearchBar";

import BasePage from "../BasePage/BasePage";

import KLogApiService from "../../Services/KLogApiService";
import Style from "./Investigate.module.css";

function Investigate() {
  const theme = useTheme();
  const { enqueueSnackbar } = useSnackbar();

  const [apps, setApps] = useState([]);
  const [selectedApps, setSelectedApps] = useState([]);
  const [logs, setLogs] = useState([]);

  // Search / filter options
  const [sourceSearch, setSourceSearch] = useState(true);
  const [subjectSearch, setSubjectSearch] = useState(true);
  const [levelSearch, setLevelSearch] = useState(true);
  const [messageSearch, setMessageSearch] = useState(true);

  const columns = [
    {
      field: "applicationId",
      headerName: "Application",
      valueGetter: (params) => {
        let app = find(apps, (a) => a.id === params.row.applicationId);
        return app.text;
      },
    },
    {
      field: "level",
      headerName: "Level",
      renderCell: (params) => {
        let text = "#000000";
        let background = "#ffffff";

        if (params.value === "Info") text = theme.palette.info.main;
        if (params.value === "Warning") text = theme.palette.warning.main;
        else if (params.value === "Error") text = theme.palette.error.main;
        else if (params.value === "Critical") {
          text = "#ffffff";
          background = theme.palette.error.main;
        }

        return (
          <div style={{ color: text, backgroundColor: background }}>
            {params.value}
          </div>
        );
      },
    },
    {
      field: "timestamp",
      headerName: "TimeStamp",
      flex: 1,
      valueGetter: (params) =>
        moment(params.row.timestamp).format("MM/DD/YYYY HH:mm:ss Z"),
    },
    {
      field: "component",
      headerName: "Component",
    },
    {
      field: "message",
      headerName: "Message",
      flex: 5,
    },
  ];

  useEffect(() => {
    const loadApps = async () => {
      try {
        let apps = await KLogApiService.Keys.getApiKeys();
        let appOptions = apps.map((app) => {
          return { id: app.applicationId, text: app.name };
        });
        let defaultSelected = appOptions.map((option, index) => index);
        setApps(appOptions);
        setSelectedApps(defaultSelected);
      } catch (e) {
        enqueueSnackbar(e.message, { variant: "error" });
      }
    };

    loadApps();
  }, [enqueueSnackbar]);

  const handleAppChange = (event) => {
    const {
      target: { value },
    } = event;
    setSelectedApps(value);
  };

  const renderSelectedApp = (selected) => {
    let selectedText = selected.map((id) => apps[id].text);
    return selectedText.join(", ");
  };

  const handleSearch = async (begin, end, text) => {
    let ids = selectedApps.map((index) => apps[index].id);
    let fields = [];
    let level = null;

    [sourceSearch, subjectSearch, levelSearch, messageSearch].forEach(
      (field, i) => {
        if (i === 0 && field) fields.push("Source");
        else if (i === 1 && field) fields.push("Subject");
        else if (i === 2 && field) {
          if (text.match("Info")) level = "Info";
          else if (text.match("Warning")) level = "Warning";
          else if (text.match("Error")) level = "Error";
          else if (text.match("Critical")) level = "Critical";
        } else if (i === 3 && field) fields.push("Message");
      }
    );

    text = level ? text.split("//")[1] : text;
    text = fields.length > 0 ? text : null;

    try {
      let results = await KLogApiService.Logs.getLogs(
        begin.format("YYYY-MM-DD HH:mm:ss Z"),
        end.format("YYYY-MM-DD HH:mm:ss Z"),
        ids.join(","),
        level,
        text,
        fields.join(",")
      );

      let logs = results.items;
      while (results.nextPageUrl) {
        results = await KLogApiService.logs.getLogPage(results.nextPageUrl);
        logs = logs.concat(results.items);
      }

      logs = logs.map((log) => {
        log.id = log.logId;
        return log;
      });

      setLogs(logs);
    } catch (e) {
      enqueueSnackbar(e.message, { variant: "error" });
    }
  };

  return (
    <BasePage
      title="Investigate"
      content={
        <div id="search_container" className={Style.base}>
          <div id="search_input">
            <LogSearchBar
              showDatePickers
              showButton
              onSearch={handleSearch}
              selectLabel="Applications"
              selectValue={selectedApps}
              selectOptions={apps}
              selectRender={renderSelectedApp}
              handleSelectChange={handleAppChange}
              searchOptions={[
                {
                  id: "source",
                  value: sourceSearch,
                  text: "Source",
                  handler: setSourceSearch,
                },
                {
                  id: "subject",
                  value: subjectSearch,
                  text: "Subject",
                  handler: setSubjectSearch,
                },
                {
                  id: "level",
                  value: levelSearch,
                  text: "Level",
                  handler: setLevelSearch,
                },
                {
                  id: "message",
                  value: messageSearch,
                  text: "Message",
                  handler: setMessageSearch,
                },
              ]}
            />
          </div>
          <div id="search_results" className={Style.flex_grow}>
            <DataGrid columns={columns} rows={logs} density="compact" />
          </div>
        </div>
      }
    />
  );
}

export default Investigate;
