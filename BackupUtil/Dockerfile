FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app

COPY *.sln .
COPY BackupUtil/*.csproj ./BackupUtil/

RUN dotnet restore BackupUtil/BackupUtil.csproj

COPY BackupUtil/. ./BackupUtil/

RUN sed -i "s|c:\\\temp\\\BackupUtil\\\|/my/logs/|g" BackupUtil/nlog.config
RUN sed -i "s|c:\\\temp\\\|/my/logs/|g" BackupUtil/nlog.config

RUN dotnet publish BackupUtil/BackupUtil.csproj -c Release -o ../out

FROM mcr.microsoft.com/dotnet/core/runtime:2.2
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "BackupUtil.dll"]