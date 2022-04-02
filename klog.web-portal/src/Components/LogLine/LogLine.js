import React from "react";
import { useTheme } from "@mui/material";

function LogLine(props) {
  const theme = useTheme();

  let text = "#000000";
  let background = "#ffffff";

  if (props.log.level === "Info") text = theme.palette.info.main;
  else if (props.log.level === "Warning") text = theme.palette.warning.main;
  else if (props.log.level === "Error") text = theme.palette.error.main;
  else if (props.log.level === "Critical") {
    text = "#ffffff";
    background = theme.palette.error.main;
  }

  return (
    <div id={props.log.logId} key={props.log.logId}>
      <span>{props.log.timestamp || " "}</span>
      <span>{props.showLevel && " || "}</span>
      <span style={{ color: text, backgroundColor: background }}>
        {props.showLevel && props.log.level}
      </span>
      <span>{props.showSource && ` || ${props.log.source}`}</span>
      <span>{props.showSubject && ` || ${props.log.subject}`}</span>
      <span>{props.showMessage && ` || ${props.log.message}`}</span>
    </div>
  );
}

export default LogLine;
