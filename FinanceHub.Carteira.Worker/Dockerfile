FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY . .

RUN dotnet restore "FinanceHub.Carteira.Worker/FinanceHub.Carteira.Worker.csproj"

RUN dotnet publish \
    "FinanceHub.Carteira.Worker/FinanceHub.Carteira.Worker.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore


FROM mcr.microsoft.com/dotnet/runtime:9.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FinanceHub.Carteira.Worker.dll"]