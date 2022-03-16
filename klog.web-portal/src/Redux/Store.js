import { configureStore } from "@reduxjs/toolkit";

import appReducer from "./AppSlice";
import notificationReducer from "./NotificationSlice";
import userReducer from "./UserSlice";

export default configureStore({
  reducer: {
    app: appReducer,
    notifications: notificationReducer,
    user: userReducer,
  },
});
