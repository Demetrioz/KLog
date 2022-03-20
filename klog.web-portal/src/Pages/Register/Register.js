import React, { useState } from "react";
import { useDispatch } from "react-redux";
import { useSnackbar } from "notistack";

import { setUser } from "../../Redux/UserSlice";
import { LOGIN_STATE, setLoginState } from "../../Redux/AppSlice";

import Button from "@mui/material/Button";
import InputAdornment from "@mui/material/InputAdornment";
import TextField from "@mui/material/TextField";

import AccountIcon from "@mui/icons-material/AccountCircle";
import BackIcon from "@mui/icons-material/ArrowBack";
import PasswordIcon from "@mui/icons-material/Lock";

import KLogApiService from "../../Services/KLogApiService";

import Logo from "../../Assets/Img/Logo.png";

import Style from "./Register.module.css";

function Register() {
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

  const handleRegister = async () => {
    try {
      let token = await KLogApiService.Auth.register(
        userFields.username,
        userFields.password
      );

      dispatch(setUser(token));
    } catch (e) {
      enqueueSnackbar(e.message, { variant: "error" });
    }
  };

  const handlePasswordDown = (event) => {
    if (event.key === "Enter") handleRegister();
  };

  const handleBack = () => {
    dispatch(setLoginState(LOGIN_STATE.LOGIN));
  };

  return (
    <div id="container" className={Style.container}>
      <div id="login_box" className={Style.login_box}>
        <img src={Logo} alt="KLog" className={Style.logo} />
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
            id="register"
            variant="contained"
            sx={{ marginTop: "12px" }}
            onClick={handleRegister}
          >
            Register
          </Button>
          <Button
            id="back"
            variant="text"
            startIcon={<BackIcon />}
            sx={{ marginTop: "12px" }}
            onClick={handleBack}
          >
            Login
          </Button>
        </div>
      </div>
    </div>
  );
}

export default Register;
