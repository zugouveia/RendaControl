FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ./RendaControl.Back.slnx ./
COPY ./NuGet.config ./
COPY ./src/RendaControl.Domain/RendaControl.Domain.csproj ./src/RendaControl.Domain/
COPY ./src/RendaControl.Application/RendaControl.Application.csproj ./src/RendaControl.Application/
COPY ./src/RendaControl.Persistence/RendaControl.Persistence.csproj ./src/RendaControl.Persistence/
COPY ./src/RendaControl.Web/RendaControl.Web.csproj ./src/RendaControl.Web/

RUN dotnet restore ./src/RendaControl.Web/RendaControl.Web.csproj --configfile ./NuGet.config

COPY ./src ./src
RUN dotnet publish ./src/RendaControl.Web/RendaControl.Web.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "RendaControl.Web.dll"]
