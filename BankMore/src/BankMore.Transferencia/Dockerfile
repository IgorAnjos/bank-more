# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar solução e todos os projetos
COPY ["BankMore.sln", "./"]
COPY ["src/BankMore.Transferencia/Domain/BankMore.Transferencia.Domain.csproj", "src/BankMore.Transferencia/Domain/"]
COPY ["src/BankMore.Transferencia/Application/BankMore.Transferencia.Application.csproj", "src/BankMore.Transferencia/Application/"]
COPY ["src/BankMore.Transferencia/Infrastructure/BankMore.Transferencia.Infrastructure.csproj", "src/BankMore.Transferencia/Infrastructure/"]
COPY ["src/BankMore.Transferencia/Api/BankMore.Transferencia.Api.csproj", "src/BankMore.Transferencia/Api/"]

# Restore
RUN dotnet restore "src/BankMore.Transferencia/Api/BankMore.Transferencia.Api.csproj"

# Copiar código fonte
COPY ["src/BankMore.Transferencia/", "src/BankMore.Transferencia/"]

# Build
WORKDIR "/src/src/BankMore.Transferencia/Api"
RUN dotnet build "BankMore.Transferencia.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "BankMore.Transferencia.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 5004

# Labels para metadata
LABEL maintainer="BankMore Team <contato@bankmore.com.br>"
LABEL version="1.0.0"
LABEL description="BankMore - API Transferência"
LABEL org.opencontainers.image.title="BankMore API Transferência"
LABEL org.opencontainers.image.version="1.0.0"
LABEL org.opencontainers.image.description="API REST para transferências entre contas"
LABEL org.opencontainers.image.vendor="BankMore"
LABEL org.opencontainers.image.licenses="MIT"

# Criar diretório para o banco de dados
RUN mkdir -p /app/data

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BankMore.Transferencia.Api.dll"]
