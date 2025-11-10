# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar solução e projeto
COPY ["BankMore.sln", "./"]
COPY ["src/BankMore.Web/BankMore.Web.csproj", "src/BankMore.Web/"]

# Restore
RUN dotnet restore "src/BankMore.Web/BankMore.Web.csproj"

# Copiar código fonte
COPY ["src/BankMore.Web/", "src/BankMore.Web/"]

# Build e Publish
WORKDIR "/src/src/BankMore.Web"
RUN dotnet build "BankMore.Web.csproj" -c Release -o /app/build
RUN dotnet publish "BankMore.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage para Blazor WebAssembly (usa nginx para servir arquivos estáticos)
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
EXPOSE 80

# Labels para metadata
LABEL maintainer="BankMore Team <contato@bankmore.com.br>"
LABEL version="1.0.0"
LABEL description="BankMore - Web Application"
LABEL org.opencontainers.image.title="BankMore Web App"
LABEL org.opencontainers.image.version="1.0.0"
LABEL org.opencontainers.image.description="Interface Blazor WebAssembly para BankMore"
LABEL org.opencontainers.image.vendor="BankMore"
LABEL org.opencontainers.image.licenses="MIT"

# Copiar arquivos publicados do Blazor WebAssembly
COPY --from=build /app/publish/wwwroot .

# Copiar configuração customizada do nginx
COPY src/BankMore.Web/nginx.conf /etc/nginx/nginx.conf
