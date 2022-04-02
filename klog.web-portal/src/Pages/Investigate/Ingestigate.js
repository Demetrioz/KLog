import React, { useEffect, useState } from "react";
import { useSnackbar } from "notistack";

import LogSearchBar from "../../Components/LogSearchBar/LogSearchBar";

import BasePage from "../BasePage/BasePage";

import KLogApiService from "../../Services/KLogApiService";

function Investigate() {
  const { enqueueSnackbar } = useSnackbar();

  const [apps, setApps] = useState([]);
  const [selectedApps, setSelectedApps] = useState([]);

  // Search / filter options
  const [sourceSearch, setSourceSearch] = useState(true);
  const [subjectSearch, setSubjectSearch] = useState(true);
  const [levelSearch, setLevelSearch] = useState(true);
  const [messageSearch, setMessageSearch] = useState(true);

  // Visibility options
  // const [source, setSource] = useState(true);
  // const [subject, setSubject] = useState(true);
  // const [level, setLevel] = useState(true);
  // const [message, setMessage] = useState(true);

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

      console.log("results", results);
    } catch (e) {
      enqueueSnackbar(e.message, { variant: "error" });
    }
  };

  return (
    <BasePage
      title="Investigate"
      content={
        <div id="search_container">
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
              // visibilityOptions={[
              //   {
              //     id: "source",
              //     value: source,
              //     text: "Source",
              //     handler: setSource,
              //   },
              //   {
              //     id: "subject",
              //     value: subject,
              //     text: "Subject",
              //     handler: setSubject,
              //   },
              //   {
              //     id: "level",
              //     value: level,
              //     text: "Level",
              //     handler: setLevel,
              //   },
              //   {
              //     id: "message",
              //     value: message,
              //     text: "Message",
              //     handler: setMessage,
              //   },
              // ]}
            />
          </div>
        </div>
      }
    />
  );
}

export default Investigate;
