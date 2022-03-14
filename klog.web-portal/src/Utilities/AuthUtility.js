import JSEncrypt from "jsencrypt";
import jwt_decode from "jwt-decode";

import KLogApiService from "../Services/KLogApiService";

class AuthUtility {
  static publicKey = null;

  static encrypt = async (data) => {
    if (this.publicKey === null) await this.setPublicKey();

    var encrypt = new JSEncrypt();
    encrypt.setPublicKey(this.publicKey);

    return encrypt.encrypt(data);
  };

  static setPublicKey = async () => {
    this.publicKey = await KLogApiService.request(
      "authentication/key",
      null,
      "POST"
    );
  };

  static getToken = async (username, password, path) => {
    let [encryptedUser, encryptedPass] = await Promise.all([
      this.encrypt(username),
      this.encrypt(password),
    ]);

    let encryptedCreds = {
      Username: encryptedUser,
      Password: encryptedPass,
    };

    let token = await KLogApiService.request(
      `authentication/${path}`,
      encryptedCreds,
      "POST"
    );

    KLogApiService.userToken = token;
    let decodedToken = jwt_decode(token);
    return decodedToken;
  };
}

export default AuthUtility;
