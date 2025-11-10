# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar solução e projeto
COPY ["BankMore.sln", "./"]
COPY ["src/BankMore.Tarifas/BankMore.Tarifas.csproj", "src/BankMore.Tarifas/"]

# Restore
RUN dotnet restore "src/BankMore.Tarifas/BankMore.Tarifas.csproj"

# Copiar código fonte
COPY ["src/BankMore.Tarifas/", "src/BankMore.Tarifas/"]

# Build
WORKDIR "/src/src/BankMore.Tarifas"
RUN dotnet build "BankMore.Tarifas.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "BankMore.Tarifas.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Labels para metadata
LABEL maintainer="BankMore Team <contato@bankmore.com.br>"
LABEL version="1.0.0"
LABEL description="BankMore - Worker Tarifas"
LABEL org.opencontainers.image.title="BankMore Worker Tarifas"
LABEL org.opencontainers.image.version="1.0.0"
LABEL org.opencontainers.image.description="Worker Service para processamento de tarifas via Kafka"
LABEL org.opencontainers.image.vendor="BankMore"
LABEL org.opencontainers.image.licenses="MIT"

# Criar diretório para o banco de dados
RUN mkdir -p /app/data

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BankMore.Tarifas.dll"]
