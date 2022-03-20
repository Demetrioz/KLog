---
sidebar_position: 1
---

# Build Process

To build the KLog project from source, follow the below steps.

## Requirements

- [Git](https://git-scm.com/)
- [Node](https://nodejs.org/en/)
- [.Net 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- [Docker](https://www.docker.com/) (optional)

## 1) Clone the repo

Clone the repo to your local machine from
[GitHub](https://github.com/KTech-Industries/KLog)

## 2) Build klog.web-portal

Navigate to the klog.web-portal directory, and run the following command:

```
npm run build
```

## 3) Build the KLog.Api project

1. Copy the /build folder from (3) to the /wwwroot folder in KLog.Api
2. Create an appsettings.Development.json and/or an appsettings.Production.json
   with the following format. PublicKey and PrivateKey can be blank for now:

```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AppSettings": {
    "ConnectionStrings": {
      "KLog.Db": "DB_CONNECTION_STRING"
    },
    "CORS": [ "URLS_OF_WEB_APP" ]
  },
  "SecuritySettings": {
    "Issuer": "YOUR_ORG_NAME",
    "Audience": "YOUR_ORG_NAME",
    "SecretKey": "25_CHAR_SECRET",
    "PublicKey": "PUBLIC_XML_RSA_KEY",
    "PrivateKey": "PRIVATE_XML_RSA_KEY"
  }
}
```

3. Perform the build

```
dotnet publish KLog.Api.csproj -c Release -o <OUTPUT_DIRECTORY>
```

## 4) Generate a public / private key pair

If you don't have a public / private key pair, you can generate one by making
a post request to the /authentication/keypair endpoint. These keys are used
in the appsettings.json file in order to encrypt and decrypt usernames and
passwords while logging in and registering as a new user.

```
POST /api/authentication/keypair

Response:
{
  requestId: <GUID>
  data: {
    publicKey: <XML_KEY>
    privateKey: <XML_KEY>
  }
  error: null
}
```

## 5) Update appsettings.json

If you havent already, add the keys generated in step 4 to your appsettings.json
file. From here, you have two options:

1. Copy the updated appsettings.json to the build folder
   (typically something like /bin/Release/net6.0)
2. Rebuild the api (step 3)

At this point, the API has been built and can be deployed via IIS, Azure App
Service, etc.

## 6) Create a docker image (optional)

If you prefer to use docker, you can create a docker image by using the provided
docker file.

```
docker build -t <TAG_NAME> .
```
