FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

COPY *.sln .
COPY BackupUtil/*.csproj ./BackupUtil/

RUN dotnet restore BackupUtil/BackupUtil.csproj

COPY BackupUtil/. ./BackupUtil/

RUN dotnet publish BackupUtil/BackupUtil.csproj -c Release -o ./out

FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app
COPY --from=build-env /app/out .

ENTRYPOINT ["dotnet", "BackupUtil.dll"]