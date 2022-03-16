import React from "react";

import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import IconButton from "@mui/material/IconButton";
import InputAdornment from "@mui/material/InputAdornment";
import TextField from "@mui/material/TextField";
import Typography from "@mui/material/Typography";

import KeyIcon from "@mui/icons-material/Key";
// import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete";

import Style from "./ApiKeyCard.module.css";

function ApiKeyCard(props) {
  const handleDelete = () => {
    if (props.onDelete) props.onDelete(props.app.applicationId);
  };

  return (
    <Card sx={{ margin: "8px 0" }}>
      <CardContent sx={{ display: "flex", justifyContent: "space-between" }}>
        <div id="app" className={Style.content_base}>
          <Typography variant="body1">{props.app.name}</Typography>
          <Typography variant="body2" color="gray">
            {props.app.id}
          </Typography>
        </div>
        <TextField
          id="api_key"
          type="password"
          value={props.app.key}
          disabled={true}
          sx={{
            "& .MuiOutlinedInput-notchedOutline": {
              border: "none",
            },
          }}
          InputProps={{
            startAdornment: (
              <InputAdornment position="start">
                <KeyIcon />
              </InputAdornment>
            ),
          }}
        />
        <div id="actions">
          {/* <IconButton aria-label="edit">
            <EditIcon />
          </IconButton> */}
          <IconButton aria-label="delete" onClick={handleDelete}>
            <DeleteIcon />
          </IconButton>
        </div>
      </CardContent>
    </Card>
  );
}

export default ApiKeyCard;
