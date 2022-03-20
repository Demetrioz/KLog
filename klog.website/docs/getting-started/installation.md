---
sidebar_position: 2
---

# Installation

There are a number of ways that you can choose to install KLog, depending on
whether you want to run the project as code, as a container, locally, or in the
cloud. Below you'll find an overview of each method. As time goes on, the goal
is to simplify the steps as much as possible.

## IIS and SQL Server

1. Create a Microsoft SQL database
2. Build the project by following the directions in [Build Process](./build.md)
3. Setup an IIS site
4. Deploy the code from step 2, to the IIS directory

## Docker

1. Complete the build directions in [Build Process](./build.md) including the
   optional step 6. The following steps will assume that the docker image was
   tagged as "KLog"
2. Create a docker network for the KLog app and a database container

```
docker network create <NETWORK_NAME>
```

3. Create a container running Microsoft SQL on our docker network

```
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=<PASSWORD>" -p 1433:1433 --name <CONTAINER_NAME> --hostname <HOST_NAME> --network <NETWORK_NAME> --network-alias <HOST_NAME> -d mcr.microsoft.com/mssql/server:2019-latest
```

4. Update the SQL password

```
docker exec -it <CONTAINER_NAME> /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P <PASSWORD> -Q "ALTER LOGIN SA WITHPASSWORD='<NEW_PASSWORD>'"
```

<em>Note: Make sure the appsettings.json in KLog.Api is using the correct
connection details for the sql server. It should be something similar to: </em>

```
Data Source=<HOST_NAME>,1433;Initial Catalog=<DATABASE_NAME>;Persist Security Info=False;User Id=SA;Password=<NEW_PASSWORD>;
```

5. [Create a security certificate for the docker image](https://github.com/dotnet/dotnet-docker/blob/main/samples/host-aspnetcore-https.md)

6. Run the KLog image created from step 6 of the [Build Process](./build.md).
   The command assumes the certificate was created as in step 5.

```
docker run -it -p 8000:80 -p 8001:443 -e ASPNETCORE_URLS="https://+;http://+" -e ASPNETCORE_HTTPS_PORT=8001 -e ASPNETCORE_Kestrel__Certificates__Default__Password="<PASSWORD_FROM_STEP_5>" -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx --name <CONTAINER_NAME> --network <NETWORK_NAME> --network-alias <HOST_NAME> -v $env:USERPROFILE\.aspnet\https:/https/ KLog
```
