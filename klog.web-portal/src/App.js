import React, { useEffect } from "react";
// import { useDispatch, useSelector } from "react-redux";
import { useSelector } from "react-redux";
import { BrowserRouter, Routes, Route } from "react-router-dom";

import Login from "./Pages/Login/Login";
import Register from "./Pages/Register/Register";

import Feed from "./Pages/Feed/Feed";

import NotFound from "./Pages/NotFound/NotFound";
import { LOGIN_STATE } from "./Redux/AppSlice";

import KLogApiService from "./Services/KLogApiService";

function App() {
  // const dispatch = useDispatch();
  const user = useSelector((state) => state.user);
  const app = useSelector((state) => state.app);

  useEffect(() => {
    if (KLogApiService.apiUrl === null) KLogApiService.initialize();

    // TODO: Check for required password resets
  });

  const loginContent =
    app.loginState === LOGIN_STATE.LOGIN ? <Login /> : <Register />;

  const appContent = (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Feed />} />
        <Route path="*" element={<NotFound />} />
      </Routes>
    </BrowserRouter>
  );

  return user.sub == null ? loginContent : appContent;
}

export default App;
