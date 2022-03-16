import { createSlice } from "@reduxjs/toolkit";
import findIndex from "lodash/findIndex";

export const notificationSlice = createSlice({
  name: "notifications",
  initialState: {
    dialogs: [],
  },
  reducers: {
    addDialog: (state, action) => {
      state.dialogs.push(action.payload);
    },
    removeDialog: (state, action) => {
      let index = findIndex(state.dialogs, (d) => {
        return d.key === action.payload;
      });

      console.log("payload:", action.payload);
      console.log("index:", index);

      if (index >= 0) state.dialogs.splice(index, 1);
    },
  },
});

export const { addDialog, removeDialog } = notificationSlice.actions;
export default notificationSlice.reducer;
