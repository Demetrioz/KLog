import { createSlice } from "@reduxjs/toolkit";

export const initialUserState = {
  aud: null,
  authentication_method: null,
  email: null,
  exp: null,
  iss: null,
  name: null,
  phone: null,
  reset_required: null,
  sub: null,
};

export const userSlice = createSlice({
  name: "user",
  initialState: initialUserState,
  reducers: {
    setUser: (state, action) => {
      Object.assign(state, action.payload);
    },
  },
});

export const { setUser } = userSlice.actions;
export default userSlice.reducer;
