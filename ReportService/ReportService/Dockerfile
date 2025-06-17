FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ReportService/ReportService.csproj ReportService/
RUN dotnet restore ReportService/ReportService.csproj

COPY . .
WORKDIR /src/ReportService
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ReportService.dll"]
