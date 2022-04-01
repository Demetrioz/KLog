import React from "react";
import { useDispatch } from "react-redux";
import { useNavigate } from "react-router";
import { useMediaQuery } from "react-responsive";

import { initialUserState, setUser } from "../../Redux/UserSlice";
import { LOGIN_STATE, setLoginState } from "../../Redux/AppSlice";

import Drawer from "@mui/material/Drawer";
import Divider from "@mui/material/Divider";
import List from "@mui/material/List";

import FeedIcon from "@mui/icons-material/Stream";
import SearchIcon from "@mui/icons-material/QueryStats";
import KeyIcon from "@mui/icons-material/Key";
import LogoutIcon from "@mui/icons-material/ExitToApp";

import NavigationButton from "../NavigationButton/NavigationButton";

import Settings from "../../Settings";

import Style from "./NavigationDrawer.module.css";
import Logo from "../../Assets/Img/Logo.png";

function NavigationDrawer(props) {
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const isNarrow = useMediaQuery({
    query: `(max-width: ${Settings.smallWidth}px)`,
  });

  const handleNavigation = (destination) => {
    navigate(`/${destination}`);

    if (props.onClose) props.onClose();
  };

  const handleLogout = () => {
    dispatch(setLoginState(LOGIN_STATE.LOGIN));
    dispatch(setUser(initialUserState));
    navigate("");
  };

  const drawerVariant = isNarrow ? "temporary" : "permanent";

  return (
    <Drawer
      variant={drawerVariant}
      open={props.open}
      onClose={props.onClose}
      sx={{
        width: 240,
        flexShrink: 0,
        [`& .MuiDrawer-paper`]: { width: 240, boxSizing: "border-box" },
      }}
    >
      <div className={Style.header}>
        <img src={Logo} alt="KLog" className={Style.logo} />
      </div>
      <Divider />
      <List>
        <NavigationButton
          icon={<FeedIcon />}
          text="Log Feed"
          onClick={() => handleNavigation("")}
        />
        <NavigationButton
          icon={<SearchIcon />}
          text="Investigate"
          onClick={() => handleNavigation("search")}
        />
        <NavigationButton
          icon={<KeyIcon />}
          text="Api Keys"
          onClick={() => handleNavigation("keys")}
        />
      </List>
      <div id="spacer" className={Style.spacer}></div>
      <List>
        <NavigationButton
          icon={<LogoutIcon />}
          text="Logout"
          onClick={handleLogout}
        />
      </List>
    </Drawer>
  );
}

export default NavigationDrawer;
