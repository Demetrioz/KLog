import React from "react";

import BasePage from "../BasePage/BasePage";

function NotFound() {
  return (
    <BasePage
      title="Invalid Page"
      content={<div>The content you're looking for does not exist.</div>}
    />
  );
}

export default NotFound;
