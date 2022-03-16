import React, { useEffect } from "react";
import { useSelector } from "react-redux";
import { BrowserRouter, Routes, Route } from "react-router-dom";

import Login from "./Pages/Login/Login";
import Register from "./Pages/Register/Register";

import Feed from "./Pages/Feed/Feed";
import ApiKeys from "./Pages/ApiKeys/ApiKeys";

import NotFound from "./Pages/NotFound/NotFound";
import { LOGIN_STATE } from "./Redux/AppSlice";

import KLogApiService from "./Services/KLogApiService";

function App() {
  const user = useSelector((state) => state.user);
  const app = useSelector((state) => state.app);
  const dialogs = useSelector((state) => state.notifications.dialogs);

  useEffect(() => {
    if (KLogApiService.apiUrl === null) KLogApiService.initialize();

    // TODO: Check for required password resets
  });

  const loginContent =
    app.loginState === LOGIN_STATE.LOGIN ? <Login /> : <Register />;

  const appContent = (
    <BrowserRouter>
      {dialogs.length > 0 &&
        dialogs.map((dialog) => {
          return (
            <React.Fragment key={dialog.key}>{dialog.content}</React.Fragment>
          );
        })}
      <Routes>
        <Route path="/" element={<Feed />} />
        <Route path="/keys" element={<ApiKeys />} />
        <Route path="*" element={<NotFound />} />
      </Routes>
    </BrowserRouter>
  );

  return user.sub == null ? loginContent : appContent;
}

export default App;
