FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["SmartTask/SmartTask.csproj", "SmartTask/"]
RUN dotnet restore "SmartTask/SmartTask.csproj"
COPY . .
WORKDIR "/src/SmartTask"
RUN dotnet build "SmartTask.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SmartTask.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartTask.dll"]