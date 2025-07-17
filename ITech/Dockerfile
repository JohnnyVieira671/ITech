# Etapa 1: Build da aplicação
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copia tudo para dentro do container
COPY . .

# Restaura as dependências
RUN dotnet restore

# Publica em modo release na pasta /app/publish
RUN dotnet publish -c Release -o /app/publish

# Etapa 2: Imagem mais leve para rodar a aplicação
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expõe porta 80
EXPOSE 80

# Inicia a aplicação
ENTRYPOINT ["dotnet", "ITech.dll"]

