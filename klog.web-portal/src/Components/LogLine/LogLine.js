import React from "react";
import { useTheme } from "@mui/material";

function LogLine(props) {
  const theme = useTheme();

  let logLevelColor = "black";
  if (props.log.level === "Error") logLevelColor = theme.palette.error.main;
  else if (props.log.level === "Warning")
    logLevelColor = theme.palette.warning.main;
  else if (props.log.level === "Info") logLevelColor = theme.palette.info.main;

  return (
    <div id={props.log.logId} key={props.log.logId}>
      <span>{props.log.timestamp || " "}</span>
      <span>{props.showLevel && " || "}</span>
      <span style={{ color: logLevelColor }}>
        {props.showLevel && props.log.level}
      </span>
      <span>{props.showSource && ` || ${props.log.source}`}</span>
      <span>{props.showSubject && ` || ${props.log.subject}`}</span>
      <span>{props.showMessage && ` || ${props.log.message}`}</span>
    </div>
  );
}

export default LogLine;
