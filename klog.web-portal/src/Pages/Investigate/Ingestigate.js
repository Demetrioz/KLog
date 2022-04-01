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
        console.log("apps:", apps);
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
  }, []);

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

  const handleSearch = (begin, end, text) => {
    let ids = selectedApps.map((index) => apps[index].id);
    console.log(`Searching the fields: source (${sourceSearch}), subject (${subjectSearch}), level (${levelSearch}), message (${messageSearch})
    of logs from apps ${ids} for ${text} from ${begin} to ${end}`);
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
