import React from "react";
import { useDispatch } from "react-redux";

import { removeDialog } from "../../Redux/NotificationSlice";

import DialogBase from "../DialogBase/DialogBase";
import IconButton from "@mui/material/IconButton";
import InputAdornment from "@mui/material/InputAdornment";
import TextField from "@mui/material/TextField";

import CopyIcon from "@mui/icons-material/CopyAllRounded";

function NewApiKeyDialog(props) {
  const dispatch = useDispatch();

  const handleClose = () => {
    dispatch(removeDialog(props.id));
  };

  const handleCopy = () => {
    navigator.clipboard.writeText(props.apiKey);
  };

  return (
    <DialogBase
      title="New Api Key"
      id={props.id}
      content={
        <div id="new_key">
          Your new key has been created! Please copy the below key, as it will
          no longer be shown to you after this, and there will be no way to
          retrieve it.
          <TextField
            id="api_key"
            label="Api Key"
            variant="outlined"
            margin="normal"
            value={props.apiKey}
            fullWidth
            InputProps={{
              endAdornment: (
                <InputAdornment position="end">
                  <IconButton onClick={handleCopy}>
                    <CopyIcon />
                  </IconButton>
                </InputAdornment>
              ),
            }}
          />
        </div>
      }
      actions={[
        {
          id: "close",
          text: "Close",
          color: "error",
          handler: handleClose,
        },
      ]}
    />
  );
}

export default NewApiKeyDialog;
