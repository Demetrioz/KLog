import React, { useEffect, useState } from "react";
import { useDispatch } from "react-redux";
import { useSnackbar } from "notistack";
import { clone, remove } from "lodash";

import { addDialog } from "../../Redux/NotificationSlice";

import Fab from "@mui/material/Fab";
import Typography from "@mui/material/Typography";

import AddIcon from "@mui/icons-material/Add";

import BasePage from "../BasePage/BasePage";
import ApiKeyCard from "../../Components/ApiKeyCard/ApiKeyCard";
import AddApiKeyDialog from "../../Components/AddApiKeyDialog/AddApiKeyDialog";
import NewApiKeyDialog from "../../Components/NewApiKeyDialog/NewApiKeyDialog";

import KLogApiService from "../../Services/KLogApiService";

function ApiKeys() {
  const dispatch = useDispatch();
  const { enqueueSnackbar } = useSnackbar();

  const [keys, setKeys] = useState([]);

  useEffect(() => {
    const loadKeys = async () => {
      try {
        let keys = await KLogApiService.Keys.getApiKeys();
        setKeys(keys);
      } catch (e) {
        enqueueSnackbar(e.message, { variant: "error" });
      }
    };

    loadKeys();
  }, [enqueueSnackbar]);

  const onSaveSuccess = (newKey) => {
    console.log("new key:", newKey);

    let updatedKeys = clone(keys);
    updatedKeys.push(newKey);
    setKeys(updatedKeys);

    dispatch(
      addDialog({
        key: "new_key",
        content: <NewApiKeyDialog id="new_key" apiKey={newKey.key} />,
      })
    );
  };

  const handleAdd = () => {
    dispatch(
      addDialog({
        key: "add_key",
        content: <AddApiKeyDialog id="add_key" onSave={onSaveSuccess} />,
      })
    );
  };

  const handleDelete = async (applicationId) => {
    try {
      let result = await KLogApiService.Keys.deleteApiKey(applicationId);
      console.log("result:", result);
      if (result) {
        let updatedKeys = clone(keys);
        remove(updatedKeys, (key) => key.applicationId === applicationId);
        setKeys(updatedKeys);
      } else
        enqueueSnackbar("Unable to delete key. Please try again", {
          variant: "warning",
        });
    } catch (e) {
      enqueueSnackbar(e.message, { variant: "error" });
    }
  };

  // const handleEdit = () => {

  // }

  const keyRows = keys.map((k) => {
    return <ApiKeyCard key={k.applicationId} app={k} onDelete={handleDelete} />;
  });

  return (
    <BasePage
      title="Api Keys"
      content={
        <div id="key_container">
          <div
            id="headers"
            style={{ display: "flex", justifyContent: "space-between" }}
          >
            <Typography variant="body1" sx={{ paddingLeft: "16px" }}>
              Application
            </Typography>
            <Typography variant="body1">
              ApiKey (hidden for security)
            </Typography>
            <Typography variant="body1" sx={{ paddingRight: "16px" }}>
              Actions
            </Typography>
          </div>
          {keyRows}
          <Fab
            color="primary"
            aria-label="add"
            onClick={handleAdd}
            sx={{ position: "absolute", bottom: "16px", right: "16px" }}
          >
            <AddIcon />
          </Fab>
        </div>
      }
    />
  );
}

export default ApiKeys;
