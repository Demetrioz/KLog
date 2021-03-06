import React from "react";
import ReactDOM from "react-dom";
import { Provider } from "react-redux";

import { ThemeProvider, createTheme } from "@mui/material/styles";
import { SnackbarProvider } from "notistack";
import Slide from "@mui/material/Slide";

import App from "./App";
import Store from "./Redux/Store";

import reportWebVitals from "./reportWebVitals";

import "./index.css";
import Settings from "./Settings";

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
  <Provider store={Store}>
    <ThemeProvider theme={theme}>
      <SnackbarProvider
        maxSnack={3}
        autoHideDuration={2000}
        TransitionComponent={Slide}
        anchorOrigin={{ vertical: "bottom", horizontal: "center" }}
      >
        <App />
      </SnackbarProvider>
    </ThemeProvider>
  </Provider>,
  document.getElementById("root")
);

document.title = `KLog - v${Settings.version}`;

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
