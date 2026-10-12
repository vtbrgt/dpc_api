# Estágio 1: Compilação (SDK)
FROM ://microsoft.com AS build
WORKDIR /src

# Copia os arquivos de projeto e restaura as dependências
COPY *.csproj ./
RUN dotnet restore

# Copia o restante do código e compila a aplicação
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Estágio 2: Execução (Runtime leve)
FROM ://microsoft.com AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Comando para iniciar a aplicação
ENTRYPOINT ["dotnet", "TaskManager.Api.dll"]