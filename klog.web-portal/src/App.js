import React from "react";
import { BrowserRouter, Routes, Route } from "react-router-dom";

import Feed from "./Pages/Feed/Feed";
import NotFound from "./Pages/NotFound/NotFound";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Feed />} />
        <Route path="*" element={<NotFound />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
