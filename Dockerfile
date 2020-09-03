FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /src
COPY ["JWTAuthentication.csproj", ""]
RUN dotnet restore "./JWTAuthentication.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "JWTAuthentication.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JWTAuthentication.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JWTAuthentication.dll"]

