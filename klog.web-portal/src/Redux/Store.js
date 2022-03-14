import { configureStore } from "@reduxjs/toolkit";

import appReducer from "./AppSlice";
import userReducer from "./UserSlice";

export default configureStore({
  reducer: {
    app: appReducer,
    user: userReducer,
  },
});
