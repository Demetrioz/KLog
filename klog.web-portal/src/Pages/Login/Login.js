import React, { useState } from "react";
import { useDispatch } from "react-redux";
import { useSnackbar } from "notistack";

import { setUser } from "../../Redux/UserSlice";
import { setLoginState, LOGIN_STATE } from "../../Redux/AppSlice";

import Button from "@mui/material/Button";
import InputAdornment from "@mui/material/InputAdornment";
import TextField from "@mui/material/TextField";

import AccountIcon from "@mui/icons-material/AccountCircle";
import PasswordIcon from "@mui/icons-material/Lock";

import KLogApiService from "../../Services/KLogApiService";

// import Logo from "../../assets/img/logo.png";

import Style from "./Login.module.css";

function Login() {
  const dispatch = useDispatch();
  const { enqueueSnackbar } = useSnackbar();
  const [userFields, setUserFields] = useState({
    username: "",
    password: "",
  });

  const handleUserChange = (event) => {
    setUserFields({
      username: event.target.value,
      password: userFields.password,
    });
  };

  const handlePasswordChange = (event) => {
    setUserFields({
      username: userFields.username,
      password: event.target.value,
    });
  };

  const handlePasswordDown = (event) => {
    if (event.key === "Enter") handleLogin();
  };

  const handleLogin = async () => {
    try {
      let token = await KLogApiService.Auth.login(
        userFields.username,
        userFields.password
      );

      dispatch(setUser(token));
    } catch (e) {
      enqueueSnackbar(e.message, { variant: "error" });
    }
  };

  const handleRegister = () => {
    dispatch(setLoginState(LOGIN_STATE.REGISTER));
  };

  // const handleForgot = () => {
  //   console.log("Forgot password!");
  // };

  return (
    <div id="container" className={Style.container}>
      <div id="login_box" className={Style.login_box}>
        {/* <img src={Logo} alt="PyFarm" className={Style.logo} /> */}
        <div id="input" className={Style.input}>
          <TextField
            id="usernname"
            label="Username"
            variant="outlined"
            margin="normal"
            value={userFields.username}
            onChange={handleUserChange}
            InputProps={{
              startAdornment: (
                <InputAdornment position="start">
                  <AccountIcon />
                </InputAdornment>
              ),
            }}
          />
          <TextField
            id="password"
            label="Password"
            variant="outlined"
            margin="normal"
            type="password"
            value={userFields.password}
            onChange={handlePasswordChange}
            onKeyDown={handlePasswordDown}
            InputProps={{
              startAdornment: (
                <InputAdornment position="start">
                  <PasswordIcon />
                </InputAdornment>
              ),
            }}
          />
          <Button
            id="login"
            variant="contained"
            sx={{ marginTop: "12px" }}
            onClick={handleLogin}
          >
            Login
          </Button>
          <Button
            id="register"
            variant="text"
            sx={{ marginTop: "12px" }}
            onClick={handleRegister}
          >
            Register
          </Button>
          {/* <Button
            id="forgot"
            variant="text"
            sx={{ marginTop: "8px" }}
            onClick={handleForgot}
          >
            Forgot Password?
          </Button> */}
        </div>
      </div>
    </div>
  );
}

export default Login;
