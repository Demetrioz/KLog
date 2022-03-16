import React, { useState } from "react";
import { useDispatch } from "react-redux";
import { useSnackbar } from "notistack";

import { removeDialog } from "../../Redux/NotificationSlice";

import DialogBase from "../DialogBase/DialogBase";

import TextField from "@mui/material/TextField";

import KLogApiService from "../../Services/KLogApiService";

function AddApiKeyDialog(props) {
  const dispatch = useDispatch();
  const { enqueueSnackbar } = useSnackbar();
  const [appName, setAppName] = useState("");

  const handleNameChange = (event) => {
    setAppName(event.target.value);
  };

  const handleKeyDown = (event) => {
    if (event.key === "Enter") handleSave();
  };

  const handleCancel = () => {
    dispatch(removeDialog(props.id));
  };

  const handleSave = async () => {
    try {
      let result = await KLogApiService.Keys.createApiKey(appName);
      console.log("result:", result);
      if (props.onSave) props.onSave(result);

      enqueueSnackbar("Key Created", { variant: "success" });
      handleCancel();
    } catch (e) {
      enqueueSnackbar(e.message, { variant: "error" });
    }
  };

  return (
    <DialogBase
      title="Create Api Key"
      id={props.id}
      content={
        <div id="add_key">
          <TextField
            id="app_name"
            label="App Name"
            variant="outlined"
            margin="normal"
            value={appName}
            onChange={handleNameChange}
            onKeyDown={handleKeyDown}
          />
        </div>
      }
      actions={[
        {
          id: "cancel",
          text: "Cancel",
          color: "secondary",
          handler: handleCancel,
        },
        {
          id: "save",
          text: "Save",
          handler: handleSave,
        },
      ]}
    />
  );
}

export default AddApiKeyDialog;
