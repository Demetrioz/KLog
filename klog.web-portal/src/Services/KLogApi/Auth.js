import AuthUtility from "../../Utilities/AuthUtility";

export const register = async (username, password) => {
  return AuthUtility.getToken(username, password, "register");
};

export const login = async (username, password) => {
  return AuthUtility.getToken(username, password, "login");
};
