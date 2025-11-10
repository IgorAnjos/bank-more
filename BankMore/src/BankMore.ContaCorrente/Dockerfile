# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar solução e todos os projetos
COPY ["BankMore.sln", "./"]
COPY ["src/BankMore.ContaCorrente/Domain/BankMore.ContaCorrente.Domain.csproj", "src/BankMore.ContaCorrente/Domain/"]
COPY ["src/BankMore.ContaCorrente/Application/BankMore.ContaCorrente.Application.csproj", "src/BankMore.ContaCorrente/Application/"]
COPY ["src/BankMore.ContaCorrente/Infrastructure/BankMore.ContaCorrente.Infrastructure.csproj", "src/BankMore.ContaCorrente/Infrastructure/"]
COPY ["src/BankMore.ContaCorrente/Api/BankMore.ContaCorrente.Api.csproj", "src/BankMore.ContaCorrente/Api/"]

# Restore
RUN dotnet restore "src/BankMore.ContaCorrente/Api/BankMore.ContaCorrente.Api.csproj"

# Copiar código fonte
COPY ["src/BankMore.ContaCorrente/", "src/BankMore.ContaCorrente/"]

# Build
WORKDIR "/src/src/BankMore.ContaCorrente/Api"
RUN dotnet build "BankMore.ContaCorrente.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "BankMore.ContaCorrente.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 5003

# Labels para metadata
LABEL maintainer="BankMore Team <contato@bankmore.com.br>"
LABEL version="1.0.0"
LABEL description="BankMore - API Conta Corrente"
LABEL org.opencontainers.image.title="BankMore API Conta Corrente"
LABEL org.opencontainers.image.version="1.0.0"
LABEL org.opencontainers.image.description="API REST para gerenciamento de contas correntes"
LABEL org.opencontainers.image.vendor="BankMore"
LABEL org.opencontainers.image.licenses="MIT"

# Criar diretório para o banco de dados
RUN mkdir -p /app/data

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BankMore.ContaCorrente.Api.dll"]
