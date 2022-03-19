FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["KLog.Api/KLog.Api.csproj", "KLog.Api/"]
COPY ["KLog.DataModel/KLog.DataModel.csproj", "KLog.DataModel/"]
COPY ["KLog.DataModel.Migrations/KLog.DataModel.Migrations.csproj", "KLog.DataModel.Migrations/"] 
RUN dotnet restore "KLog.Api/KLog.Api.csproj"
COPY ["KLog.Api/", "KLog.Api/"]
COPY ["KLog.DataModel/", "KLog.DataModel/"]
COPY ["KLog.DataModel.Migrations/", "KLog.DataModel.Migrations/"]
WORKDIR "/src/KLog.Api"
RUN dotnet build "KLog.Api.csproj" -c Release -o /app/build

FROM build as publish
RUN dotnet publish "KLog.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "KLog.Api.dll" ]