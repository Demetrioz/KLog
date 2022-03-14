import { createSlice } from "@reduxjs/toolkit";

export const LOGIN_STATE = {
  LOGIN: 0,
  REGISTER: 1,
};

export const appSlice = createSlice({
  name: "app",
  initialState: {
    loginState: LOGIN_STATE.LOGIN,
  },
  reducers: {
    setLoginState: (state, action) => {
      state.loginState = action.payload;
    },
  },
});

export const { setLoginState } = appSlice.actions;
export default appSlice.reducer;
