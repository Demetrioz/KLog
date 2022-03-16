import React from "react";
import { useDispatch } from "react-redux";

import { removeDialog } from "../../Redux/NotificationSlice";

import Button from "@mui/material/Button";
import Dialog from "@mui/material/Dialog";
import DialogTitle from "@mui/material/DialogTitle";
import DialogContent from "@mui/material/DialogContent";
import DialogActions from "@mui/material/DialogActions";

function DialogBase(props) {
  const dispatch = useDispatch();

  const handleClose = () => {
    if (props.onClose) props.onClose();

    if (!props.isBlocking) dispatch(removeDialog(props.id));
  };

  return (
    <div id="container">
      <Dialog open={true} onClose={handleClose}>
        {props.title && <DialogTitle>{props.title}</DialogTitle>}
        <DialogContent>{props.content}</DialogContent>
        {props.actions && (
          <DialogActions>
            {props.actions.map((action, index) => {
              return (
                <Button
                  key={action.id}
                  id={action.id ?? `${index}`}
                  variant={action.variant ?? "contained"}
                  onClick={action.handler}
                  color={action.color ?? "primary"}
                >
                  {action.text}
                </Button>
              );
            })}
          </DialogActions>
        )}
      </Dialog>
    </div>
  );
}

export default DialogBase;
