FROM mcr.microsoft.com/dotnet/sdk:8.0 as build

WORKDIR /src
COPY . .
RUN dotnet restore Vonk.Facade.Starter.sln
RUN dotnet build Vonk.Facade.Starter.sln -c Release -o /app/build
RUN dotnet publish Vonk.Facade.Starter.sln -c Release -o /app/publish

FROM firely/server

WORKDIR /app
COPY deploy/firelyserver-license.json .
COPY deploy/appsettings.instance.json .
RUN sed -i "s/Password=<PASSWORD>/Password=$_DB_PASSWORD/g" appsettings.instance.json
RUN cat appsettings.instance.json

COPY --from=build /app/publish/Visi.Repository.dll plugins/.
COPY --from=build /app/publish/Oracle.ManagedDataAccess.dll plugins/.
COPY --from=build /app/publish/Oracle.EntityFrameworkCore.dll plugins/.
COPY --from=build /app/publish/System.DirectoryServices.Protocols.dll plugins/.

