import React from "react";
import { useNavigate } from "react-router";
import { useMediaQuery } from "react-responsive";

import AppBar from "@mui/material/AppBar";
import IconButton from "@mui/material/IconButton";
import Toolbar from "@mui/material/Toolbar";
import Typography from "@mui/material/Typography";

import MenuIcon from "@mui/icons-material/Menu";
import BackIcon from "@mui/icons-material/ArrowBack";

import Settings from "../../Settings";

function Header(props) {
  const navigate = useNavigate();

  const isNarrow = useMediaQuery({
    query: `(max-width: ${Settings.smallWidth}px)`,
  });

  const handleMenuClick = () => {
    if (props.onMenuClick) props.onMenuClick();
    else console.log("Provide an onMenuClick prop");
  };

  const handleBackClick = () => {
    navigate(props.backUrl);
  };

  const clickAction = props.showBackButton ? handleBackClick : handleMenuClick;

  const buttonIcon = props.showBackButton ? <BackIcon /> : <MenuIcon />;

  return (
    <AppBar position="fixed" sx={{ zIndex: 1201 }}>
      <Toolbar>
        {isNarrow && (
          <IconButton size="large" color="inherit" onClick={clickAction}>
            {buttonIcon}
          </IconButton>
        )}
        <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
          {props.title ?? "Provide a Title"}
        </Typography>
      </Toolbar>
    </AppBar>
  );
}

export default Header;
