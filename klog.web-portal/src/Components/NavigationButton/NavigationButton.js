import React from "react";

import ListItem from "@mui/material/ListItem";
import ListItemIcon from "@mui/material/ListItemIcon";
import ListItemText from "@mui/material/ListItemText";

function NavigationButton(props) {
  const handleClick = () => {
    if (props.onClick) props.onClick();
    else console.log("provide an onClick prop");
  };

  return (
    <ListItem button key={props.text} onClick={handleClick}>
      <ListItemIcon>{props.icon}</ListItemIcon>
      <ListItemText primary={props.text} />
    </ListItem>
  );
}

export default NavigationButton;
