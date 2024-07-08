FROM mcr.microsoft.com/dotnet/sdk:8.0 as build

WORKDIR /src
COPY Facade.props /.
COPY Visi.Repository/Visi.Repository.csproj .
RUN dotnet restore Visi.Repository.csproj
COPY Visi.Repository/ .
RUN dotnet build Visi.Repository.csproj -c Release -o /app/build
FROM build AS publish
RUN dotnet publish Visi.Repository.csproj -c Release -o /app/publish

FROM firely/server

WORKDIR /app
COPY deploy/firelyserver-license.json .
COPY deploy/appsettings.instance.json .
COPY --from=publish /app/publish/Visi.Repository.dll plugins/.
COPY --from=publish /app/publish/Oracle.ManagedDataAccess.dll plugins/.
COPY --from=publish /app/publish/Oracle.EntityFrameworkCore.dll plugins/.
COPY --from=publish /app/publish/System.DirectoryServices.Protocols.dll plugins/.

