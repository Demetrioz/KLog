import React from "react";
import ReactDOM from "react-dom";

import { ThemeProvider, createTheme } from "@mui/material/styles";
import { SnackbarProvider } from "notistack";
import Slide from "@mui/material/Slide";

import App from "./App";

import reportWebVitals from "./reportWebVitals";

import "./index.css";

const theme = createTheme({
  palette: {
    // mode: "dark",
    primary: {
      main: "#060606",
    },
    secondary: {
      main: "#e27c16",
    },
    // divider: "rgba(0,0,0,0.12)",
    // background: {
    //   paper: "#717171",
    //   default: "#646464",
    // },
    // text: {
    //   primary: "#ffffff",
    // },
  },
});

ReactDOM.render(
  <ThemeProvider theme={theme}>
    <SnackbarProvider
      maxSnack={3}
      autoHideDuration={2000}
      TransitionComponent={Slide}
      anchorOrigin={{ vertical: "bottom", horizontal: "center" }}
    >
      <App />
    </SnackbarProvider>
  </ThemeProvider>,
  document.getElementById("root")
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
